using Domain.Interfaces.States;
using Domain.Patterns.State;
using Fast_Bank.Domain.Utils;
using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    using System.ComponentModel.DataAnnotations;

    public abstract class Cuenta
    {
        [Key]
        public string NumeroCuenta { get; private set; }
        public double Saldo { get; private set; }
        public DateTime FechaApertura { get; private set; }

        // Propiedad persistida por EF Core para guardar el estado en la BD
        public string Estado { get; private set; }

        private IEstadoCuenta _estado;

        // Colección de movimientos gestionada por el Agregado.
        // EF Core usa el campo _movimientos como backing field.
        protected List<Movimiento> _movimientos = new();
        public IReadOnlyCollection<Movimiento> Movimientos => _movimientos.AsReadOnly();

        // Navigation to the owning cliente (optional)
        public Cliente? Cliente { get; private set; }

        // Foreign key property for the one-to-one relationship with Cliente
        public string? ClienteCedula { get; private set; }

        protected Cuenta()
        {
            NumeroCuenta = string.Empty;
            Saldo = 0.0;
            FechaApertura = DateTime.UtcNow;
            Estado = "Activa";
            _estado = ResolverEstado(Estado);
        }

        protected Cuenta(string numeroCuenta, double saldoInicial, IEstadoCuenta estadoInicial)
        {
            NumeroCuenta = numeroCuenta;
            Saldo = saldoInicial;
            FechaApertura = DateTime.UtcNow;
            _estado = estadoInicial;
            Estado = estadoInicial.Nombre;
        }

        private static IEstadoCuenta ResolverEstado(string nombreEstado)
        {
            return nombreEstado switch
            {
                "Bloqueada" => new EstadoCuentaBloqueada(),
                _ => new EstadoCuentaActiva(),
            };
        }

        public void ReconstruirEstadoDesdeBD()
        {
            _estado = ResolverEstado(Estado);
        }

        internal void SetCliente(Cliente cliente)
        {
            Cliente = cliente ?? throw new ArgumentNullException(nameof(cliente));
            ClienteCedula = cliente.Cedula;
        }

        public void CambiarEstado(IEstadoCuenta nuevoEstado)
        {
            _estado = nuevoEstado;
            Estado = nuevoEstado.Nombre;
        }


        internal void ModificarSaldo(double monto)
        {
            Saldo = FinancialRounding.RoundMoney(Saldo + monto);
        }

        public void Depositar(double monto)
        {
            _estado.Depositar(this, monto);
        }

        public virtual void AplicarInteresMensual()
        {
        }

        protected void AplicarMontoInteres(double monto)
        {
            if (monto == 0.0) return;
            ModificarSaldo(monto);
        }

        public virtual void Retirar(double monto)
        {
            _estado.Retirar(this, monto);
        }
    }
}
