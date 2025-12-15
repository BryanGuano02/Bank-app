
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
}
