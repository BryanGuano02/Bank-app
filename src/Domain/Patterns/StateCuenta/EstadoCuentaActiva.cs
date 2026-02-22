using System;
using Domain.Entities;
using Domain.Interfaces.States;

namespace Domain.Patterns.State;

public class EstadoCuentaActiva : IEstadoCuenta
{
    public string Nombre => "Activa";
    public void Depositar(Cuenta cuenta, double monto)
    {
        if (monto <= 0) throw new ArgumentOutOfRangeException(nameof(monto), "El monto debe ser positivo.");

        // Llamamos al método interno de la cuenta para actualizar saldo
        cuenta.ModificarSaldo(monto);
    }

    public void Retirar(Cuenta cuenta, double monto)
    {
        if (monto <= 0) throw new ArgumentOutOfRangeException(nameof(monto), "El monto debe ser positivo.");

        cuenta.ModificarSaldo(-monto);
    }

    public void Transferir(Cuenta cuenta, Cuenta destino, double monto)
    {
        // Reutilizamos la lógica de retiro para el origen
        Retirar(cuenta, monto);

        // Y depositamos en el destino (asumiendo que el destino también sabe depositarse)
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
