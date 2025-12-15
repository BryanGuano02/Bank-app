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
            string cedulaCliente,
            string numeroCuenta,
            decimal saldoInicial,
            double tasaInteres)
        {
            if (string.IsNullOrWhiteSpace(cedulaCliente))
                throw new ArgumentException("Cédula inválida.", nameof(cedulaCliente));

            if (string.IsNullOrWhiteSpace(numeroCuenta))
                throw new ArgumentException("Número de cuenta inválido.", nameof(numeroCuenta));

            var cliente = await _context.Clientes.FindAsync(cedulaCliente);
            if (cliente == null)
                throw new InvalidOperationException("Cliente no encontrado.");

            var existente = await _context.Cuentas.FindAsync(numeroCuenta);
            if (existente != null)
                throw new InvalidOperationException("La cuenta ya existe.");

            var cuenta = cliente.CrearCuentaAhorros(
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
            string cedulaCliente,
            string numeroCuenta,
            decimal saldoInicial,
            decimal limiteSobregiro)
        {
            if (string.IsNullOrWhiteSpace(cedulaCliente))
                throw new ArgumentException("Cédula inválida.", nameof(cedulaCliente));

            if (string.IsNullOrWhiteSpace(numeroCuenta))
                throw new ArgumentException("Número de cuenta inválido.", nameof(numeroCuenta));

            var cliente = await _context.Clientes.FindAsync(cedulaCliente);
            if (cliente == null)
                throw new InvalidOperationException("Cliente no encontrado.");

            var existente = await _context.Cuentas.FindAsync(numeroCuenta);
            if (existente != null)
                throw new InvalidOperationException("La cuenta ya existe.");

            var cuenta = cliente.CrearCuentaCorriente(
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
