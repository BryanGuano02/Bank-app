using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Fast_Bank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using DomainInteresesService = Domain.Services.InteresesService;

namespace Fast_Bank.Application.Services
{
    public class InteresesUseCase
    {
        private readonly IDdContext _context;
        private readonly DomainInteresesService _domainInteresesService = new();

        public InteresesUseCase(IDdContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<AcreditacionInteresesResult> AcreditarInteresesMensualesAsync()
        {
            var resultado = new AcreditacionInteresesResult();

            var cuentasAhorros = await _context.CuentasAhorros
                .Where(c => c.Saldo > 0) // Solo cuentas con saldo positivo
                .ToListAsync();

            foreach (var cuenta in cuentasAhorros)
            {
                try
                {
                    // Calcular inter�s
                    var montoInteres = _domainInteresesService.CalcularInteresMensual(cuenta);

                    // Si el inter�s es mayor a cero, acreditarlo
                    if (montoInteres > 0)
                    {
                        var movimiento = _domainInteresesService.CrearYEjecutarAcreditacionInteres(
                            Guid.NewGuid().ToString(),
                            cuenta,
                            montoInteres
                        );

                        await _context.Movimientos.AddAsync(movimiento);

                        resultado.CuentasProcesadas++;
                        resultado.MontoTotalAcreditado += montoInteres;
                        resultado.DetallesPorCuenta.Add(new DetalleAcreditacion
                        {
                            NumeroCuenta = cuenta.NumeroCuenta,
                            SaldoAnterior = cuenta.Saldo - montoInteres,
                            MontoInteres = montoInteres,
                            SaldoNuevo = cuenta.Saldo,
                            TasaAplicada = cuenta.TasaInteres
                        });
                    }
                    else
                    {
                        resultado.CuentasOmitidas++;
                    }
                }
                catch (Exception ex)
                {
                    resultado.Errores.Add($"Error en cuenta {cuenta.NumeroCuenta}: {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();

            return resultado;
        }

        // DTOs de respuesta
        public class AcreditacionInteresesResult
        {
            public int CuentasProcesadas { get; set; }
            public int CuentasOmitidas { get; set; }
            public decimal MontoTotalAcreditado { get; set; }
            public List<string> Errores { get; set; } = new();
            public List<DetalleAcreditacion> DetallesPorCuenta { get; set; } = new();
        }

        public class DetalleAcreditacion
        {
            public string NumeroCuenta { get; set; } = string.Empty;
            public decimal SaldoAnterior { get; set; }
            public decimal MontoInteres { get; set; }
            public decimal SaldoNuevo { get; set; }
            public double TasaAplicada { get; set; }
        }

        public class SimulacionInteresResult
        {
            public string NumeroCuenta { get; set; } = string.Empty;
            public decimal SaldoActual { get; set; }
            public double TasaInteresAnual { get; set; }
            public double TasaInteresMensual { get; set; }
            public decimal InteresCalculado { get; set; }
            public decimal SaldoProyectado { get; set; }
        }

        // Métodos para intereses de sobregiro en cuentas corrientes
        public async Task<AcreditacionInteresSobregiroResult> AcreditarInteresSobregiroATodas()
        {
            var resultado = new AcreditacionInteresSobregiroResult();

            var cuentasCorrientes = await _context.CuentasCorrientes
                .Where(c => c.Saldo < 0) // Solo cuentas en sobregiro
                .ToListAsync();

            foreach (var cuenta in cuentasCorrientes)
            {
                try
                {
                    // Calcular interés de sobregiro
                    var montoInteres = _domainInteresesService.CalcularInteresSobregiro(cuenta);

                    // Si el interés es mayor a cero, cobrarlo
                    if (montoInteres > 0)
                    {
                        var movimiento = _domainInteresesService.CrearYEjecutarCargoInteresSobregiro(
                            Guid.NewGuid().ToString(),
                            cuenta,
                            montoInteres
                        );

                        await _context.Movimientos.AddAsync(movimiento);

                        resultado.CuentasProcesadas++;
                        resultado.MontoTotalCobrado += montoInteres;
                        resultado.DetallesPorCuenta.Add(new DetalleCobro
                        {
                            NumeroCuenta = cuenta.NumeroCuenta,
                            SaldoAnterior = cuenta.Saldo + montoInteres, // Era menos negativo
                            MontoInteres = montoInteres,
                            SaldoNuevo = cuenta.Saldo,
                            TasaAplicada = cuenta.InteresSobregiro
                        });
                    }
                    else
                    {
                        resultado.CuentasOmitidas++;
                    }
                }
                catch (Exception ex)
                {
                    resultado.Errores.Add($"Error en cuenta {cuenta.NumeroCuenta}: {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();

            return resultado;
        }

        // DTOs adicionales para sobregiro
        public class AcreditacionInteresSobregiroResult
        {
            public int CuentasProcesadas { get; set; }
            public int CuentasOmitidas { get; set; }
            public decimal MontoTotalCobrado { get; set; }
            public List<string> Errores { get; set; } = new();
            public List<DetalleCobro> DetallesPorCuenta { get; set; } = new();
        }

        public class DetalleCobro
        {
            public string NumeroCuenta { get; set; } = string.Empty;
            public decimal SaldoAnterior { get; set; }
            public decimal MontoInteres { get; set; }
            public decimal SaldoNuevo { get; set; }
            public decimal TasaAplicada { get; set; }
        }
    }
}
