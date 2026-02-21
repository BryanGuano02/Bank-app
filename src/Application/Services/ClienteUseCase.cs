using Domain.Entities;
using Domain.Enums;
using Fast_Bank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using DomainClienteService = Domain.Services.ClienteService;

namespace Fast_Bank.Application.Services;

public class ClienteUseCase
{
    private readonly IDdContext _context;
    private readonly DomainClienteService _domainClienteService;

    public ClienteUseCase(IDdContext context, DomainClienteService domainClienteService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _domainClienteService = domainClienteService ?? throw new ArgumentNullException(nameof(domainClienteService));
    }

    public async Task<List<Cliente>> GetAllClientesAsync()
    {
        return await _context.Clientes
            .Include(c => c.Cuenta)
            .ToListAsync();
    }

    public async Task<Cliente?> GetClienteAsync(string cedula)
    {
        if (string.IsNullOrWhiteSpace(cedula)) throw new ArgumentException("Cédula inválida.", nameof(cedula));

        return await _context.Clientes
            .Include(c => c.Cuenta)
            .FirstOrDefaultAsync(c => c.Cedula == cedula);
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

    public async Task<Cliente> CrearClienteConCuentaCorrienteAsync(
        string cedula, string nombre, string apellido, string direccion, string correo, string telefono,
        decimal saldoInicial)
    {
        if (string.IsNullOrWhiteSpace(cedula)) throw new ArgumentException("Cédula inválida.", nameof(cedula));

        var existeCliente = await _context.Clientes.FindAsync(cedula);
        if (existeCliente != null) throw new InvalidOperationException("El cliente ya existe.");

        var cliente = _domainClienteService.CrearClienteConCuentaCorriente(
            cedula, nombre, apellido, direccion, correo, telefono,
            saldoInicial);

        await _context.Clientes.AddAsync(cliente);
        await _context.SaveChangesAsync();

        return cliente;
    }

    public async Task<Cliente> CrearClienteConCuentaAhorrosAsync(
        string cedula, string nombre, string apellido, string direccion, string correo, string telefono,
        decimal saldoInicial)
    {
        if (string.IsNullOrWhiteSpace(cedula)) throw new ArgumentException("Cédula inválida.", nameof(cedula));

        var existeCliente = await _context.Clientes.FindAsync(cedula);
        if (existeCliente != null) throw new InvalidOperationException("El cliente ya existe.");

        var cliente = _domainClienteService.CrearClienteConCuentaAhorros(
            cedula, nombre, apellido, direccion, correo, telefono,
            saldoInicial);

        await _context.Clientes.AddAsync(cliente);
        await _context.SaveChangesAsync();

        return cliente;
    }

    // Nuevo método unificado para crear cliente con cuenta usando enum
    public async Task<Cliente> CrearClienteConCuentaAsync(
        string cedula, string nombre, string apellido, string direccion, string correo, string telefono,
        decimal saldoInicial, TipoCuenta tipoCuenta)
    {
        if (string.IsNullOrWhiteSpace(cedula)) throw new ArgumentException("Cédula inválida.", nameof(cedula));

        var existeCliente = await _context.Clientes.FindAsync(cedula);
        if (existeCliente != null) throw new InvalidOperationException("El cliente ya existe.");

        Cliente cliente;

        switch (tipoCuenta)
        {
            case TipoCuenta.Ahorros:
                cliente = _domainClienteService.CrearClienteConCuentaAhorros(
                    cedula, nombre, apellido, direccion, correo, telefono,
                    saldoInicial);
                break;

            case TipoCuenta.Corriente:
                cliente = _domainClienteService.CrearClienteConCuentaCorriente(
                    cedula, nombre, apellido, direccion, correo, telefono,
                    saldoInicial);
                break;

            default:
                throw new ArgumentException("Tipo de cuenta no válido.", nameof(tipoCuenta));
        }

        await _context.Clientes.AddAsync(cliente);
        await _context.SaveChangesAsync();

        return cliente;
    }

    // Nuevo método para crear cliente con cuenta y tarjeta de crédito
    public async Task<Cliente> CrearClienteConCuentaYTarjetaAsync(
        string cedula, string nombre, string apellido, string direccion, string correo, string telefono,
        decimal saldoInicial, TipoCuenta tipoCuenta,
        string numeroTarjeta, double limiteCredito, double tasaInteresMensual)
    {
        if (string.IsNullOrWhiteSpace(cedula)) throw new ArgumentException("Cédula inválida.", nameof(cedula));
        if (string.IsNullOrWhiteSpace(numeroTarjeta)) throw new ArgumentException("Número de tarjeta inválido.", nameof(numeroTarjeta));
        if (limiteCredito <= 0) throw new ArgumentException("Límite de crédito debe ser mayor a cero.", nameof(limiteCredito));

        var existeCliente = await _context.Clientes.FindAsync(cedula);
        if (existeCliente != null) throw new InvalidOperationException("El cliente ya existe.");

        // Verificar si el número de tarjeta ya existe
        var existeTarjeta = await _context.TarjetasCredito.FindAsync(numeroTarjeta);
        if (existeTarjeta != null) throw new InvalidOperationException("El número de tarjeta ya existe.");

        // Crear cliente con cuenta
        Cliente cliente;
        switch (tipoCuenta)
        {
            case TipoCuenta.Ahorros:
                cliente = _domainClienteService.CrearClienteConCuentaAhorros(
                    cedula, nombre, apellido, direccion, correo, telefono,
                    saldoInicial);
                break;

            case TipoCuenta.Corriente:
                cliente = _domainClienteService.CrearClienteConCuentaCorriente(
                    cedula, nombre, apellido, direccion, correo, telefono,
                    saldoInicial);
                break;

            default:
                throw new ArgumentException("Tipo de cuenta no válido.", nameof(tipoCuenta));
        }

        // Crear tarjeta de crédito
        var tarjeta = TarjetaCredito.Create(
            numeroTarjeta,
            limiteCredito,
            DateTime.Now,
            DateTime.Now.AddYears(3),
            tasaInteresMensual);

        tarjeta.IdCliente = cedula;
        _domainClienteService.AsignarTarjetaCredito(cliente, tarjeta);

        await _context.Clientes.AddAsync(cliente);
        await _context.TarjetasCredito.AddAsync(tarjeta);
        await _context.SaveChangesAsync();

        return cliente;
    }
}
