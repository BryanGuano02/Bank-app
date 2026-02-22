using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Fast_Bank.Domain.Utils;
using Fast_Bank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using DomainInteresesService = Domain.Services.InteresesService;
using Domain.Logic;

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
                    // Calcular interés esperado (usado para registro y validación)
                    var montoInteres = _domainInteresesService.CalcularInteresMensual(cuenta);
                    montoInteres = FinancialRounding.RoundMoney(montoInteres);

                    // Si el interés es mayor a cero, aplicarlo en la entidad y registrar el movimiento
                    if (montoInteres > 0)
                    {
                        var saldoAnterior = cuenta.Saldo;

                        // Aplicar el interés en la entidad (usa la lógica de CuentaAhorros)
                        cuenta.AplicarInteresMensual();

                        // Crear el movimiento de acreditación pero NO ejecutar, porque ya aplicamos el saldo
                        var movimiento = Movimiento.Create(
                            Guid.NewGuid().ToString(),
                            montoInteres,
                            null,
                            cuenta,
                            $"Interés mensual - Tasa: {CuentaAhorros.TASA_INTERES_AHORROS:P2}",
                            new InteresTipo()
                        );

                        await _context.Movimientos.AddAsync(movimiento);

                        resultado.CuentasProcesadas++;
                        resultado.MontoTotalAcreditado = FinancialRounding.RoundMoney(resultado.MontoTotalAcreditado + montoInteres);
                        resultado.DetallesPorCuenta.Add(new DetalleAcreditacion
                        {
                            NumeroCuenta = cuenta.NumeroCuenta,
                            SaldoAnterior = saldoAnterior,
                            MontoInteres = montoInteres,
                            SaldoNuevo = cuenta.Saldo,
                            TasaAplicada = CuentaAhorros.TASA_INTERES_AHORROS
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
            public double MontoTotalAcreditado { get; set; }
            public List<string> Errores { get; set; } = new();
            public List<DetalleAcreditacion> DetallesPorCuenta { get; set; } = new();
        }

        public class DetalleAcreditacion
        {
            public string NumeroCuenta { get; set; } = string.Empty;
            public double SaldoAnterior { get; set; }
            public double MontoInteres { get; set; }
            public double SaldoNuevo { get; set; }
            public double TasaAplicada { get; set; }
        }

        public class SimulacionInteresResult
        {
            public string NumeroCuenta { get; set; } = string.Empty;
            public double SaldoActual { get; set; }
            public double TasaInteresAnual { get; set; }
            public double TasaInteresMensual { get; set; }
            public double InteresCalculado { get; set; }
            public double SaldoProyectado { get; set; }
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
                    var montoInteres = _domainInteresesService.CalcularInteresSobregiro(cuenta);
                    montoInteres = FinancialRounding.RoundMoney(montoInteres);

                    if (montoInteres > 0)
                    {
                        var saldoAnterior = cuenta.Saldo;

                        // Aplicar el cargo de interés en la entidad (CuentaCorriente) - el método resta el interés
                        cuenta.AplicarInteresMensual();

                        // Registrar movimiento sin ejecutar (ya se aplicó)
                        var movimiento = Movimiento.Create(
                            Guid.NewGuid().ToString(),
                            montoInteres,
                            null,
                            cuenta,
                            $"Interés por sobregiro - Tasa: {cuenta.InteresSobregiro:P2}",
                            new InteresTipo()
                        );

                        await _context.Movimientos.AddAsync(movimiento);

                        resultado.CuentasProcesadas++;
                        resultado.MontoTotalCobrado = FinancialRounding.RoundMoney(resultado.MontoTotalCobrado + montoInteres);
                        resultado.DetallesPorCuenta.Add(new DetalleCobro
                        {
                            NumeroCuenta = cuenta.NumeroCuenta,
                            SaldoAnterior = saldoAnterior,
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
            public double MontoTotalCobrado { get; set; }
            public List<string> Errores { get; set; } = new();
            public List<DetalleCobro> DetallesPorCuenta { get; set; } = new();
        }

        public class DetalleCobro
        {
            public string NumeroCuenta { get; set; } = string.Empty;
            public double SaldoAnterior { get; set; }
            public double MontoInteres { get; set; }
            public double SaldoNuevo { get; set; }
            public double TasaAplicada { get; set; }
        }
    }
}
