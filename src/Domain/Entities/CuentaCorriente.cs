
using Domain.Interfaces.States;
using Fast_Bank.Domain.Utils;
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

        public override void AplicarInteresMensual()
        {
            // Interés de sobregiro: InteresSobregiro está en formato decimal (ej. 0.22m => 22%)
            decimal tasaMensual = InteresSobregiro / 12m;
            if (tasaMensual <= 0m) return;

            // Aplicar interés sólo si hay saldo negativo (se cobrá en sobregiro)
            if (Saldo < 0m)
            {
                decimal montoSobregiro = Math.Abs(Saldo);
                decimal montoInteres = montoSobregiro * tasaMensual;
                montoInteres = FinancialRounding.RoundMoney(montoInteres);
                if (montoInteres == 0m) return;

                // Cargar el interés (resta al saldo)
                AplicarMontoInteres(-montoInteres);
            }
        }
    }
}
