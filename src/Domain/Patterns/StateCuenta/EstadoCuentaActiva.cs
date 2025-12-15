using System;
using Domain.Entities;
using Domain.Interfaces.States;

namespace Domain.Patterns.State;

public class EstadoCuentaActiva : IEstadoCuenta
{
    public void Depositar(Cuenta cuenta, decimal monto)
    {
        if (monto <= 0) throw new ArgumentOutOfRangeException(nameof(monto), "El monto debe ser positivo.");

        // Llamamos al método interno de la cuenta para actualizar saldo
        cuenta.ModificarSaldo(monto);
    }

    public void Retirar(Cuenta cuenta, decimal monto)
    {
        if (monto <= 0) throw new ArgumentOutOfRangeException(nameof(monto), "El monto debe ser positivo.");

        // NO validar saldo aquí porque cada tipo de cuenta tiene sus propias reglas:
        // - CuentaAhorros: no permite saldo negativo
        // - CuentaCorriente: permite sobregiro hasta un límite
        // La validación ya se hizo en CuentaAhorros.Retirar() o CuentaCorriente.Retirar()

        // No generic saldo validation here: each account type handles its own validation logic.
        // if (cuenta.Saldo < monto) throw new InvalidOperationException("Saldo insuficiente.");

        cuenta.ModificarSaldo(-monto);
    }

    public void Transferir(Cuenta cuenta, Cuenta destino, decimal monto)
    {
        // Reutilizamos la lógica de retiro para el origen
        Retirar(cuenta, monto);

        // Y depositamos en el destino (asumiendo que el destino también sabe depositarse)
        // Nota: En un diseño puro, el destino también debería usar su propio State.
        destino.Depositar(monto);
    }

    public void Activar(Cuenta cuenta)
    {
        // Ya está activa
    }

    public void Bloquear(Cuenta cuenta)
    {
        // Transición de estado: Activa -> Bloqueada
        cuenta.CambiarEstado(new EstadoCuentaBloqueada());
    }
}
