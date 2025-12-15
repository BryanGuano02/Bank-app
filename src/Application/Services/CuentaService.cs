using System;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Patterns.State;
using Fast_Bank.Infrastructure.Persistence;

namespace Fast_Bank.Application.Services
{
    public class CuentaService
    {
        private readonly IDdContext _context;

        public CuentaService(IDdContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<CuentaAhorros> CrearCuentaAhorrosAsync(
            string numeroCuenta, 
            decimal saldoInicial, 
            double tasaInteres)
        {
            if (string.IsNullOrWhiteSpace(numeroCuenta))
                throw new ArgumentException("Número de cuenta inválido.", nameof(numeroCuenta));

            var existente = await _context.Cuentas.FindAsync(numeroCuenta);
            if (existente != null)
                throw new InvalidOperationException("La cuenta ya existe.");

            var cuenta = CuentaAhorros.Create(
                numeroCuenta,
                saldoInicial,
                tasaInteres,
                new EstadoCuentaActiva()
            );

            await _context.Cuentas.AddAsync(cuenta);
            await _context.SaveChangesAsync();

            return cuenta;
        }

        public async Task<CuentaCorriente> CrearCuentaCorrienteAsync(
            string numeroCuenta, 
            decimal saldoInicial, 
            decimal limiteSobregiro)
        {
            if (string.IsNullOrWhiteSpace(numeroCuenta))
                throw new ArgumentException("Número de cuenta inválido.", nameof(numeroCuenta));

            var existente = await _context.Cuentas.FindAsync(numeroCuenta);
            if (existente != null)
                throw new InvalidOperationException("La cuenta ya existe.");

            var cuenta = CuentaCorriente.Create(
                numeroCuenta,
                saldoInicial,
                limiteSobregiro,
                new EstadoCuentaActiva()
            );

            await _context.Cuentas.AddAsync(cuenta);
            await _context.SaveChangesAsync();

            return cuenta;
        }
    }
}