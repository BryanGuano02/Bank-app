
using Domain.Interfaces.States;
using System;

namespace Domain.Entities
{
    public class CuentaAhorros : Cuenta
    {
        public const double TASA_INTERES_AHORROS = 3.0; // 3%
        public double TasaInteres { get; private set; }

        // Parameterless constructor for EF Core
        protected CuentaAhorros() : base()
        {
            TasaInteres = TASA_INTERES_AHORROS;
        }

        public CuentaAhorros(string numeroCuenta, decimal saldoInicial, IEstadoCuenta estadoInicial)
            : base(numeroCuenta, saldoInicial, estadoInicial)
        {
            TasaInteres = TASA_INTERES_AHORROS;
        }

        public static CuentaAhorros Create(string numeroCuenta, decimal saldoInicial, IEstadoCuenta estadoInicial)
        {
            if (string.IsNullOrWhiteSpace(numeroCuenta)) throw new ArgumentException("Número de cuenta inválido.", nameof(numeroCuenta));
            if (estadoInicial == null) throw new ArgumentNullException(nameof(estadoInicial));

            return new CuentaAhorros(numeroCuenta, saldoInicial, estadoInicial);
        }

        public override void Retirar(decimal monto)
        {
            if (Saldo - monto < 0)
                throw new InvalidOperationException("Fondos insuficientes.");
            base.Retirar(monto);
        }
    }
}
