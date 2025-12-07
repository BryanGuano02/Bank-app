
using Domain.Interfaces.States;
using System;

namespace Domain.Entities
{
    public class CuentaAhorros : Cuenta
    {
        public double TasaInteres { get; private set; }

        public CuentaAhorros(string numeroCuenta, decimal saldoInicial, double tasaInteres, IEstadoCuenta estadoInicial)
            : base(numeroCuenta, saldoInicial, estadoInicial)
        {
            TasaInteres = tasaInteres;
        }

        public static CuentaAhorros Create(string numeroCuenta, decimal saldoInicial, double tasaInteres, IEstadoCuenta estadoInicial)
        {
            if (string.IsNullOrWhiteSpace(numeroCuenta)) throw new ArgumentException("Número de cuenta inválido.", nameof(numeroCuenta));
            if (tasaInteres < 0) throw new ArgumentOutOfRangeException(nameof(tasaInteres), "Tasa de interés no puede ser negativa.");
            if (estadoInicial == null) throw new ArgumentNullException(nameof(estadoInicial));

            return new CuentaAhorros(numeroCuenta, saldoInicial, tasaInteres, estadoInicial);
        }

        public override void Retirar(decimal monto)
        {
            if (Saldo - monto < 0)
                throw new InvalidOperationException("Fondos insuficientes.");
            base.Retirar(monto);
        }
    }
}
