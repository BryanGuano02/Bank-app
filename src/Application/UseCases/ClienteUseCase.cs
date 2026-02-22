using Domain.Entities;
using Domain.Enums;
using Fast_Bank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using DomainClienteService = Domain.Services.ClienteService;
using DomainTarjetaCreditoService = Domain.Services.TarjetaCreditoService;

namespace Fast_Bank.Application.Services;

public class ClienteUseCase
{
    private readonly IDdContext _context;
    private readonly DomainClienteService _domainClienteService;
    private readonly DomainTarjetaCreditoService _domainTarjetaCreditoService;

    public ClienteUseCase(
        IDdContext context,
        DomainClienteService domainClienteService,
        DomainTarjetaCreditoService domainTarjetaCreditoService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _domainClienteService = domainClienteService ?? throw new ArgumentNullException(nameof(domainClienteService));
        _domainTarjetaCreditoService = domainTarjetaCreditoService ?? throw new ArgumentNullException(nameof(domainTarjetaCreditoService));
    }

    public async Task<List<Cliente>> GetAllClientesAsync()
    {
        return await _context.Clientes
            .Include(c => c.Cuenta)
            .Include(c => c.TarjetaCredito)
            .ToListAsync();
    }

    public async Task<Cliente?> GetClienteAsync(string cedula)
    {
        if (string.IsNullOrWhiteSpace(cedula)) throw new ArgumentException("Cédula inválida.", nameof(cedula));

        return await _context.Clientes
            .Include(c => c.Cuenta)
            .Include(c => c.TarjetaCredito)
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
        double saldoInicial)
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
        double saldoInicial)
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
        double saldoInicial, TipoCuenta tipoCuenta)
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
        double saldoInicial, TipoCuenta tipoCuenta)
    {
        if (string.IsNullOrWhiteSpace(cedula)) throw new ArgumentException("Cédula inválida.", nameof(cedula));

        var existeCliente = await _context.Clientes.FindAsync(cedula);
        if (existeCliente != null) throw new InvalidOperationException("El cliente ya existe.");

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

        // Crear tarjeta de crédito con número generado automáticamente y valores por defecto
        var tarjeta = _domainTarjetaCreditoService.CrearTarjetaCredito(cedula);

        _domainClienteService.AsignarTarjetaCredito(cliente, tarjeta);

        await _context.Clientes.AddAsync(cliente);
        await _context.TarjetasCredito.AddAsync(tarjeta);
        await _context.SaveChangesAsync();

        return cliente;
    }
}
