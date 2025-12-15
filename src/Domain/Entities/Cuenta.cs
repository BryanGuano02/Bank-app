using Domain.Interfaces.States;
using Domain.Patterns.State;
using System;

namespace Domain.Entities
{
    using System.ComponentModel.DataAnnotations;

    public abstract class Cuenta
    {
        [Key]
        public string NumeroCuenta { get; private set; }
        public decimal Saldo { get; private set; }
        public DateTime FechaApertura { get; private set; }

        private IEstadoCuenta _estado;

        // Navigation to the owning cliente (optional)
        public Cliente? Cliente { get; private set; }

        // Foreign key property for the one-to-one relationship with Cliente
        public string? ClienteCedula { get; private set; }

        // Parameterless constructor for EF Core
        protected Cuenta()
        {
            NumeroCuenta = string.Empty;
            Saldo = 0m;
            FechaApertura = DateTime.UtcNow;
            // Assign a sensible default state when EF materializes the entity
            _estado = new EstadoCuentaActiva();
        }

        protected Cuenta(string numeroCuenta, decimal saldoInicial, IEstadoCuenta estadoInicial)
        {
            NumeroCuenta = numeroCuenta;
            Saldo = saldoInicial;
            FechaApertura = DateTime.UtcNow;
            _estado = estadoInicial;
        }

        internal void SetCliente(Cliente cliente)
        {
            Cliente = cliente ?? throw new ArgumentNullException(nameof(cliente));
            ClienteCedula = cliente.Cedula;
        }

        public void CambiarEstado(IEstadoCuenta nuevoEstado)
        {
            _estado = nuevoEstado;
        }

        internal void ModificarSaldo(decimal monto)
        {
            Saldo += monto;
        }

        public void Depositar(decimal monto)
        {
            _estado.Depositar(this, monto);
        }

        public virtual void Retirar(decimal monto)
        {
            _estado.Retirar(this, monto);
        }
    }
}
