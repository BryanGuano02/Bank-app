using System;
using System.Threading;
using System.Threading.Tasks;
using Fast_Bank.Application.Services;
using Fast_Bank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fast_Bank.API.BackgroundServices
{
    public class InteresesBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InteresesBackgroundService> _logger;

        public InteresesBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<InteresesBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Servicio de Intereses Mensuales iniciado - Verificación diaria a las 00:01 UTC");

            while (!stoppingToken.IsCancellationRequested)
            {
                var ahora = DateTime.UtcNow;
                
                // Calcular la próxima ejecución a las 00:01 UTC
                // Si aun no son las 00:01 de hoy, ejecutar hoy. Si no, ejecutar mañana.
                // Usamos < (no <=) para evitar re-ejecución inmediata si ahora es exactamente 00:01
                var horaObjetivo = ahora.Date.AddMinutes(1); // 00:01 de hoy
                var proximaEjecucion = ahora < horaObjetivo 
                    ? horaObjetivo 
                    : horaObjetivo.AddDays(1); // 00:01 del día siguiente
                var tiempoEspera = proximaEjecucion - ahora;

                _logger.LogInformation("Próxima verificación programada para: {ProximaEjecucion} UTC (en {Horas}h {Minutos}m)",
                    proximaEjecucion.ToString("yyyy-MM-dd HH:mm:ss"),
                    (int)tiempoEspera.TotalHours,
                    tiempoEspera.Minutes);

                try
                {
                    // Esperar hasta las 00:01 del día siguiente
                    await Task.Delay(tiempoEspera, stoppingToken);
                    
                    // Ejecutar la verificación
                    await VerificarYAcreditarInteresesAsync();
                }
                catch (OperationCanceledException)
                {
                    // El servicio está siendo detenido
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en el servicio de intereses");
                    // Esperar 1 hora antes de reintentar en caso de error
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
            }
        }

        private async Task VerificarYAcreditarInteresesAsync()
        {
            var ahora = DateTime.UtcNow;

            // Solo ejecutar el día 1 de cada mes
            if (ahora.Day != 1)
            {
                _logger.LogInformation("Hoy no es día 1 del mes - Omitiendo acreditación");
                return;
            }

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DdContext>();
                
                try
                {
                    // Buscar el registro de control en la BD
                    var control = await context.ControlEjecuciones
                        .FirstOrDefaultAsync(c => c.Proceso == "AcreditacionInteresesMensuales");

                    if (control != null && 
                        control.UltimaEjecucion.Year == ahora.Year && 
                        control.UltimaEjecucion.Month == ahora.Month)
                    {
                        _logger.LogInformation("Intereses ya acreditados este mes ({Year}-{Month})", 
                            ahora.Year, ahora.Month);
                        return;
                    }

                    // Intentar actualizar el registro de control PRIMERO para adquirir el "lock" optimista
                    if (control == null)
                    {
                        control = new Domain.Entities.ControlEjecucion
                        {
                            Proceso = "AcreditacionInteresesMensuales",
                            UltimaEjecucion = ahora
                        };
                        await context.ControlEjecuciones.AddAsync(control);
                    }
                    else
                    {
                        control.UltimaEjecucion = ahora;
                    }

                    // Guardar ANTES de acreditar intereses - esto actúa como un lock optimista
                    await context.SaveChangesAsync();

                    // Solo si llegamos aquí (SaveChanges exitoso), procedemos a acreditar
                    _logger.LogInformation("Iniciando acreditación automática de intereses mensuales para {Year}-{Month}", 
                        ahora.Year, ahora.Month);

                    var interesesService = scope.ServiceProvider.GetRequiredService<InteresesService>();
                    var resultado = await interesesService.AcreditarInteresesMensualesAsync();

                    _logger.LogInformation(
                        "Intereses acreditados exitosamente:\n" +
                        "   Cuentas procesadas: {CuentasProcesadas}\n" +
                        "   Monto total: ${MontoTotal:N2}\n" +
                        "   Cuentas omitidas: {CuentasOmitidas}",
                        resultado.CuentasProcesadas,
                        resultado.MontoTotalAcreditado,
                        resultado.CuentasOmitidas);
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Otra instancia actualizó el registro de control primero
                    // Los intereses NO se acreditan en esta instancia ya que fallamos antes del procesamiento
                    _logger.LogInformation(
                        "Otra instancia del servicio está procesando o ya procesó los intereses para {Year}-{Month}. " +
                        "Esta instancia no duplicará la acreditación.",
                        ahora.Year, ahora.Month);
                }
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Servicio de Intereses detenido");
            return base.StopAsync(cancellationToken);
        }
    }
}