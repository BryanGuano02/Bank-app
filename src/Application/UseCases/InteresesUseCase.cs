using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Fast_Bank.Application.DTOs.Interes;
using Fast_Bank.Domain.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;
using DomainInteresesService = Domain.Services.InteresesService;

namespace Fast_Bank.Application.Services
{
    public class InteresesUseCase
    {
        private readonly ICuentaAhorroRepository _cuentaAhorroRepository;
        private readonly ICuentaCorrienteRepository _cuentaCorrienteRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly DomainInteresesService _domainInteresesService;

        private const int BatchSize = 1000;

        public InteresesUseCase(
            ICuentaAhorroRepository cuentaAhorroRepository,
            ICuentaCorrienteRepository cuentaCorrienteRepository,
            IUnitOfWork unitOfWork,
            DomainInteresesService domainInteresesService)
        {
            _cuentaAhorroRepository = cuentaAhorroRepository ?? throw new ArgumentNullException(nameof(cuentaAhorroRepository));
            _cuentaCorrienteRepository = cuentaCorrienteRepository ?? throw new ArgumentNullException(nameof(cuentaCorrienteRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _domainInteresesService = domainInteresesService ?? throw new ArgumentNullException(nameof(domainInteresesService));
        }

        public async Task<AcreditacionInteresesResult> AcreditarInteresesMensualesAsync()
        {
            var resultado = new AcreditacionInteresesResult();
            int page = 1;
            bool hasMoreData = true;

            while (hasMoreData)
            {
                var cuentasBatch = await _cuentaAhorroRepository.GetCuentasConSaldoPositivoAsync(page, BatchSize);

                if (!cuentasBatch.Any())
                    break;

                foreach (var cuenta in cuentasBatch)
                {
                    try
                    {
                        // 1. El Domain Service hace el cálculo complejo
                        var montoInteres = _domainInteresesService.CalcularInteresMensual(cuenta);
                        montoInteres = FinancialRounding.RoundMoney(montoInteres);

                        // 2. La Entidad aplica las reglas de negocio, altera su estado y crea el Movimiento
                        var detalleAcreditacion = cuenta.AplicarInteresMensual(montoInteres);

                        // 3. El Caso de Uso solo reacciona al resultado para armar el reporte
                        if (detalleAcreditacion != null)
                        {
                            resultado.CuentasProcesadas++;
                            resultado.MontoTotalAcreditado += montoInteres;
                            resultado.DetallesPorCuenta.Add(detalleAcreditacion);
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

                await _unitOfWork.SaveChangesAsync();
                page++;

                hasMoreData = cuentasBatch.Count() == BatchSize;
            }

            resultado.MontoTotalAcreditado = FinancialRounding.RoundMoney(resultado.MontoTotalAcreditado);
            return resultado;
        }

        public async Task<AcreditacionInteresSobregiroResult> AcreditarInteresSobregiroATodas()
        {
            var resultado = new AcreditacionInteresSobregiroResult();
            int page = 1;
            bool hasMoreData = true;

            while (hasMoreData)
            {
                var cuentasBatch = await _cuentaCorrienteRepository.GetCuentasEnSobregiroAsync(page, BatchSize);

                if (!cuentasBatch.Any())
                    break;

                foreach (var cuenta in cuentasBatch)
                {
                    try
                    {
                        // 1. El Domain Service hace el cálculo complejo
                        var montoInteres = _domainInteresesService.CalcularInteresSobregiro(cuenta);
                        montoInteres = FinancialRounding.RoundMoney(montoInteres);

                        // 2. La Entidad aplica las reglas de negocio, altera su estado y crea el Movimiento
                        var detalleCobro = cuenta.AplicarInteresSobregiro(montoInteres);

                        // 3. El Caso de Uso solo reacciona al resultado para armar el reporte
                        if (detalleCobro != null)
                        {
                            resultado.CuentasProcesadas++;
                            resultado.MontoTotalCobrado += montoInteres;
                            resultado.DetallesPorCuenta.Add(detalleCobro);
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

                await _unitOfWork.SaveChangesAsync();
                page++;

                hasMoreData = cuentasBatch.Count() == BatchSize;
            }

            resultado.MontoTotalCobrado = FinancialRounding.RoundMoney(resultado.MontoTotalCobrado);
            return resultado;
        }
    }
}
