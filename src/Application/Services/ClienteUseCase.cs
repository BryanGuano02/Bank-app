using Domain.Entities;
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
        return await _context.Clientes.ToListAsync();
    }

    public async Task<Cliente?> GetClienteAsync(string cedula)
    {
        if (string.IsNullOrWhiteSpace(cedula)) throw new ArgumentException("Cédula inválida.", nameof(cedula));

        return await _context.Clientes.FindAsync(cedula);
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
        decimal saldoInicial, double tasaInteres)
    {
        if (string.IsNullOrWhiteSpace(cedula)) throw new ArgumentException("Cédula inválida.", nameof(cedula));

        var existeCliente = await _context.Clientes.FindAsync(cedula);
        if (existeCliente != null) throw new InvalidOperationException("El cliente ya existe.");

        var cliente = _domainClienteService.CrearClienteConCuentaAhorros(
            cedula, nombre, apellido, direccion, correo, telefono,
            saldoInicial, tasaInteres);

        await _context.Clientes.AddAsync(cliente);
        await _context.SaveChangesAsync();

        return cliente;
    }
}
