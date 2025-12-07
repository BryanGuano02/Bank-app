using System;
using Domain.Entities;
using Domain.Interfaces.States;

namespace Domain.Patterns.State;

public class EstadoCuentaBloqueada : IEstadoCuenta
{
    public void Depositar(Cuenta cuenta, decimal monto)
    {
        throw new InvalidOperationException("No se pueden realizar depósitos en una cuenta bloqueada.");
    }

    public void Retirar(Cuenta cuenta, decimal monto)
    {
        throw new InvalidOperationException("No se pueden realizar retiros de una cuenta bloqueada.");
    }

    public void Transferir(Cuenta cuenta, Cuenta destino, decimal monto)
    {
        throw new InvalidOperationException("No se pueden realizar transferencias desde una cuenta bloqueada.");
    }

    public void Bloquear(Cuenta cuenta)
    {
        // Ya está bloqueada, no hacemos nada o lanzamos advertencia
    }

    public void Activar(Cuenta cuenta)
    {
        // Transición de estado: Bloqueada -> Activa
        cuenta.CambiarEstado(new EstadoCuentaActiva());
    }
}
