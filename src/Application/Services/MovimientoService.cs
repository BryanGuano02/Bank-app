using System;
using System.Threading.Tasks;
using System;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Services;
using Fast_Bank.Infrastructure.Persistence;

namespace Fast_Bank.Application.Services;

public class MovimientoService
{
    private readonly IDdContext _context;
    private readonly Domain.Services.MovimientoService _domainMovimientoService = new();

    public MovimientoService(IDdContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<string> DepositarAsync(string numeroCuentaDestino, decimal monto, string descripcion)
    {
        if (string.IsNullOrWhiteSpace(numeroCuentaDestino)) throw new ArgumentException("Número de cuenta destino inválido.", nameof(numeroCuentaDestino));
        if (monto <= 0) throw new ArgumentOutOfRangeException(nameof(monto), "El monto debe ser mayor que cero.");

        var destino = await _context.Cuentas.FindAsync(numeroCuentaDestino);
        if (destino == null) throw new InvalidOperationException("Cuenta destino no encontrada.");

        var movimiento = _domainMovimientoService.CrearYEjecutarDeposito(Guid.NewGuid().ToString(), destino, monto, descripcion ?? string.Empty);

        await _context.Movimientos.AddAsync(movimiento);
        await _context.SaveChangesAsync();

        return movimiento.IdMovimiento;
    }
    public async Task<string> RetirarAsync(string numeroCuentaOrigen, decimal monto, string descripcion)
    {
        if (string.IsNullOrWhiteSpace(numeroCuentaOrigen)) throw new ArgumentException("Número de cuenta origen inválido.", nameof(numeroCuentaOrigen));
        if (monto <= 0) throw new ArgumentOutOfRangeException(nameof(monto), "El monto debe ser mayor que cero.");

        var origen = await _context.Cuentas.FindAsync(numeroCuentaOrigen);
        if (origen == null) throw new InvalidOperationException("Cuenta origen no encontrada.");

        var movimiento = _domainMovimientoService.CrearYEjecutarRetiro(Guid.NewGuid().ToString(), origen, monto, descripcion ?? string.Empty);

        await _context.Movimientos.AddAsync(movimiento);
        await _context.SaveChangesAsync();

        return movimiento.IdMovimiento;
    }
}

