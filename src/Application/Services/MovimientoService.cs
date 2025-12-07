using System;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Logic;
using Fast_Bank.Infrastructure.Persistence;

namespace Fast_Bank.Application.Services;

public class MovimientoService
{
    private readonly IDdContext _context;

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

        var movimiento = Movimiento.Create(Guid.NewGuid().ToString(), monto, null, destino, descripcion ?? string.Empty, new DepositoTipo());

        // Ejecutar la lógica del dominio (aplica el depósito en la entidad Cuenta)
        movimiento.Ejecutar();

        // Persistir movimiento y cambios en la cuenta
        await _context.Movimientos.AddAsync(movimiento);
        await _context.SaveChangesAsync();

        return movimiento.IdMovimiento;
    }
}

