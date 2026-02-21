
using Domain.Interfaces.States;
using System;

namespace Domain.Entities
{
    public class CuentaCorriente : Cuenta
    {
        public const decimal TASA_INTERES_SOBREGIRO = 0.22m; // 22%
        public decimal LimiteSobregiro { get; private set; } = 200m;
        public decimal InteresSobregiro { get; private set; } = TASA_INTERES_SOBREGIRO;

        // Parameterless constructor for EF Core
        protected CuentaCorriente() : base()
        {
        }

        public CuentaCorriente(string numeroCuenta, decimal saldoInicial, IEstadoCuenta estadoInicial)
            : base(numeroCuenta, saldoInicial, estadoInicial)
        {
        }

        public static CuentaCorriente Create(string numeroCuenta, decimal saldoInicial, IEstadoCuenta estadoInicial)
        {
            if (string.IsNullOrWhiteSpace(numeroCuenta)) throw new ArgumentException("Número de cuenta inválido.", nameof(numeroCuenta));
            if (estadoInicial == null) throw new ArgumentNullException(nameof(estadoInicial));

            return new CuentaCorriente(numeroCuenta, saldoInicial, estadoInicial);
        }

        public override void Retirar(decimal monto)
        {
            if ((Saldo + LimiteSobregiro) - monto < 0)
                throw new InvalidOperationException("Excede límite de sobregiro.");

            base.Retirar(monto);
        }
    }
}
