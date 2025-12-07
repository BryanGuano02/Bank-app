using System;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Logic;
using Fast_Bank.Infrastructure.Persistence;

namespace Fast_Bank.Application.Services;

public class MovimientoService
{
    private readonly DdContext _context;

    public MovimientoService(DdContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Realiza un depósito en la cuenta indicada y persiste el movimiento.
    /// </summary>
    public async Task<string> DepositarAsync(string numeroCuentaDestino, decimal monto, string descripcion)
    {
        if (string.IsNullOrWhiteSpace(numeroCuentaDestino)) throw new ArgumentException("Número de cuenta destino inválido.", nameof(numeroCuentaDestino));
        if (monto <= 0) throw new ArgumentOutOfRangeException(nameof(monto), "El monto debe ser mayor que cero.");

        var destino = await _context.Cuentas.FindAsync(numeroCuentaDestino);
        if (destino == null) throw new InvalidOperationException("Cuenta destino no encontrada.");

        // Para depósitos externos dejamos la cuenta origen como null.
        Cuenta? origen = null;

        var movimiento = Movimiento.Create(Guid.NewGuid().ToString(), monto, origen, destino, descripcion ?? string.Empty, new DepositoTipo());

        // Ejecutar la lógica del dominio (aplica el depósito en la entidad Cuenta)
        movimiento.Ejecutar();

        // Persistir movimiento y cambios en la cuenta
        await _context.Movimientos.AddAsync(movimiento);
        await _context.SaveChangesAsync();

        return movimiento.IdMovimiento;
    }
}

