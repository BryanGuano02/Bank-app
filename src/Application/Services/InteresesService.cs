using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Fast_Bank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fast_Bank.Application.Services
{
    public class InteresesService
    {
        private readonly IDdContext _context;
        private readonly Domain.Services.InteresesService _domainInteresesService = new();

        public InteresesService(IDdContext context)
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

        public async Task<DetalleAcreditacion> AcreditarInteresACuentaAsync(string numeroCuenta)
        {
            if (string.IsNullOrWhiteSpace(numeroCuenta))
                throw new ArgumentException("N�mero de cuenta inv�lido.", nameof(numeroCuenta));

            var cuenta = await _context.CuentasAhorros
                .FirstOrDefaultAsync(c => c.NumeroCuenta == numeroCuenta);

            if (cuenta == null)
                throw new InvalidOperationException($"No se encontr� cuenta de ahorros con n�mero: {numeroCuenta}");

            var saldoAnterior = cuenta.Saldo;
            var montoInteres = _domainInteresesService.CalcularInteresMensual(cuenta);

            if (montoInteres <= 0)
                throw new InvalidOperationException("El monto de inter�s calculado es cero o negativo.");

            var movimiento = _domainInteresesService.CrearYEjecutarAcreditacionInteres(
                Guid.NewGuid().ToString(),
                cuenta,
                montoInteres
            );

            await _context.Movimientos.AddAsync(movimiento);
            await _context.SaveChangesAsync();

            return new DetalleAcreditacion
            {
                NumeroCuenta = cuenta.NumeroCuenta,
                SaldoAnterior = saldoAnterior,
                MontoInteres = montoInteres,
                SaldoNuevo = cuenta.Saldo,
                TasaAplicada = cuenta.TasaInteres
            };
        }

        public async Task<SimulacionInteresResult> SimularInteresAsync(string numeroCuenta)
        {
            if (string.IsNullOrWhiteSpace(numeroCuenta))
                throw new ArgumentException("N�mero de cuenta inv�lido.", nameof(numeroCuenta));

            var cuenta = await _context.CuentasAhorros
                .FirstOrDefaultAsync(c => c.NumeroCuenta == numeroCuenta);

            if (cuenta == null)
                throw new InvalidOperationException($"No se encontr� cuenta de ahorros con n�mero: {numeroCuenta}");

            var interesMensual = _domainInteresesService.CalcularInteresMensual(cuenta);

            return new SimulacionInteresResult
            {
                NumeroCuenta = cuenta.NumeroCuenta,
                SaldoActual = cuenta.Saldo,
                TasaInteresAnual = cuenta.TasaInteres,
                TasaInteresMensual = cuenta.TasaInteres / 12,
                InteresCalculado = interesMensual,
                SaldoProyectado = cuenta.Saldo + interesMensual
            };
        }
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
}
