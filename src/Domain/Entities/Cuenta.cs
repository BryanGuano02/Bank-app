using Domain.Interfaces.States;
using System;

namespace Domain.Entities
{
    public abstract class Cuenta
    {
        public string NumeroCuenta { get; private set; }
        public decimal Saldo { get; private set; }
        public DateTime FechaApertura { get; private set; }
        
        private IEstadoCuenta _estado;

        protected Cuenta(string numeroCuenta, decimal saldoInicial, IEstadoCuenta estadoInicial)
        {
            NumeroCuenta = numeroCuenta;
            Saldo = saldoInicial;
            FechaApertura = DateTime.UtcNow;
            _estado = estadoInicial;
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
