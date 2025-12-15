
using System;
using System.Threading.Tasks;
using Domain.Entities;
using Fast_Bank.Infrastructure.Persistence;
using DomainClienteService = Domain.Services.ClienteService;

namespace Fast_Bank.Application.Services;

public class ClienteService
{
    private readonly IDdContext _context;
    private readonly DomainClienteService _domainClienteService;

    public ClienteService(IDdContext context, DomainClienteService domainClienteService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _domainClienteService = domainClienteService ?? throw new ArgumentNullException(nameof(domainClienteService));
    }

    public async Task<Cliente> CrearClienteAsync(string cedula, string nombre, string apellido, string direccion, string correo, string telefono)
    {
        if (string.IsNullOrWhiteSpace(cedula)) throw new ArgumentException("Cédula inválida.", nameof(cedula));

        var existente = await _context.Clientes.FindAsync(cedula);
        if (existente != null) throw new InvalidOperationException("El cliente ya existe.");

        var cliente = _domainClienteService.CrearCliente(cedula, nombre, apellido, direccion, correo, telefono);

        await _context.Clientes.AddAsync(cliente);
        await _context.SaveChangesAsync();

        return cliente;
    }

    public async Task<Cliente> ActualizarClienteAsync(string cedula, string nombre, string apellido, string direccion, string correo, string telefono)
    {
        if (string.IsNullOrWhiteSpace(cedula)) throw new ArgumentException("Cédula inválida.", nameof(cedula));

        var cliente = await _context.Clientes.FindAsync(cedula);
        if (cliente == null) throw new InvalidOperationException("Cliente no encontrado.");

        cliente = _domainClienteService.ActualizarCliente(cliente, nombre, apellido, direccion, correo, telefono);

        await _context.SaveChangesAsync();

        return cliente;
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
        if (cliente.Cuenta != null)
            throw new InvalidOperationException("El cliente ya tiene una cuenta.");

        var cuenta = _domainClienteService.CrearCuentaAhorros(cliente, numeroCuenta, saldoInicial, tasaInteres);

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
        if (cliente.Cuenta != null)
            throw new InvalidOperationException("El cliente ya tiene una cuenta.");

        var cuenta = _domainClienteService.CrearCuentaCorriente(cliente, numeroCuenta, saldoInicial, limiteSobregiro);

        await _context.Cuentas.AddAsync(cuenta);
        await _context.SaveChangesAsync();

        return cuenta;
    }
}
