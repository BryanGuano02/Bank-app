using System;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Fast_Bank.Application.DTOs.Interes;
using Fast_Bank.Domain.Utils;
using Fast_Bank.Infrastructure.Persistence;

namespace Fast_Bank.Application.Services
{
    public class CuentaUseCase
    {
        private readonly IDdContext _context;
        private readonly ICuentaAhorroRepository _cuentaAhorroRepository;
        private readonly ICuentaCorrienteRepository _cuentaCorrienteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CuentaUseCase(
            IDdContext context,
            ICuentaAhorroRepository cuentaAhorroRepository,
            ICuentaCorrienteRepository cuentaCorrienteRepository,
            IUnitOfWork unitOfWork)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cuentaAhorroRepository = cuentaAhorroRepository ?? throw new ArgumentNullException(nameof(cuentaAhorroRepository));
            _cuentaCorrienteRepository = cuentaCorrienteRepository ?? throw new ArgumentNullException(nameof(cuentaCorrienteRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Acredita intereses mensuales a todas las cuentas de ahorros con saldo positivo.
        /// La fórmula es: Saldo * (TasaAnual% / 12 / 100).
        /// </summary>
        public async Task<AcreditacionInteresesResult> AcreditarInteresesMensualesAsync()
        {
            var resultado = new AcreditacionInteresesResult();

            var cuentas = await _cuentaAhorroRepository.GetCuentasConSaldoPositivoAsync();

            foreach (var cuenta in cuentas)
            {
                try
                {
                    // Calcular interés: la tasa anual está en porcentaje (ej. 3.0 => 3%)
                    var montoInteres = FinancialRounding.RoundMoney(
                        cuenta.Saldo * (CuentaAhorros.TASA_INTERES_AHORROS / (12.0 * 100.0)));

                    // La entidad aplica las reglas de negocio, modifica su estado y crea el Movimiento
                    var detalle = cuenta.AplicarInteresMensual(montoInteres);

                    if (detalle != null)
                    {
                        resultado.CuentasProcesadas++;
                        resultado.MontoTotalAcreditado += montoInteres;
                        resultado.DetallesPorCuenta.Add(detalle);
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
            resultado.MontoTotalAcreditado = FinancialRounding.RoundMoney(resultado.MontoTotalAcreditado);
            return resultado;
        }

        /// <summary>
        /// Cobra intereses de sobregiro a todas las cuentas corrientes con saldo negativo.
        /// La fórmula es: |Saldo| * (TasaAnual / 12).
        /// </summary>
        public async Task<AcreditacionInteresSobregiroResult> AcreditarInteresSobregiroATodas()
        {
            var resultado = new AcreditacionInteresSobregiroResult();

            var cuentas = await _cuentaCorrienteRepository.GetCuentasEnSobregiroAsync();

            foreach (var cuenta in cuentas)
            {
                try
                {
                    // Calcular interés: la tasa anual está en formato decimal (ej. 0.22 => 22%)
                    var montoInteres = FinancialRounding.RoundMoney(
                        Math.Abs(cuenta.Saldo) * (cuenta.InteresSobregiro / 12.0));

                    // La entidad aplica las reglas de negocio, modifica su estado y crea el Movimiento
                    var detalle = cuenta.AplicarInteresSobregiro(montoInteres);

                    if (detalle != null)
                    {
                        resultado.CuentasProcesadas++;
                        resultado.MontoTotalCobrado += montoInteres;
                        resultado.DetallesPorCuenta.Add(detalle);
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
            resultado.MontoTotalCobrado = FinancialRounding.RoundMoney(resultado.MontoTotalCobrado);
            return resultado;
        }
    }
}
