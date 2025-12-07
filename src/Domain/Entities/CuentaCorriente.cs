
using Domain.Interfaces.States;
using System;

namespace Domain.Entities
{
    public class CuentaCorriente : Cuenta
    {
        public decimal LimiteSobregiro { get; private set; }

        // Parameterless constructor for EF Core
        protected CuentaCorriente() : base()
        {
            LimiteSobregiro = 0m;
        }

        public CuentaCorriente(string numeroCuenta, decimal saldoInicial, decimal limiteSobregiro, IEstadoCuenta estadoInicial)
            : base(numeroCuenta, saldoInicial, estadoInicial)
        {
            LimiteSobregiro = limiteSobregiro;
        }

        public static CuentaCorriente Create(string numeroCuenta, decimal saldoInicial, decimal limiteSobregiro, IEstadoCuenta estadoInicial)
        {
            if (string.IsNullOrWhiteSpace(numeroCuenta)) throw new ArgumentException("Número de cuenta inválido.", nameof(numeroCuenta));
            if (limiteSobregiro < 0) throw new ArgumentOutOfRangeException(nameof(limiteSobregiro), "Límite de sobregiro no puede ser negativo.");
            if (estadoInicial == null) throw new ArgumentNullException(nameof(estadoInicial));

            return new CuentaCorriente(numeroCuenta, saldoInicial, limiteSobregiro, estadoInicial);
        }

        public override void Retirar(decimal monto)
        {
            if ((Saldo + LimiteSobregiro) - monto < 0)
                throw new InvalidOperationException("Excede límite de sobregiro.");

            base.Retirar(monto);
        }
    }
}
