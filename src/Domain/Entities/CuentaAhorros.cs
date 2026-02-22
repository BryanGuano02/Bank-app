
using Domain.Interfaces.States;
using Fast_Bank.Domain.Utils;
using System;

namespace Domain.Entities
{
    public class CuentaAhorros : Cuenta
    {
        public const double TASA_INTERES_AHORROS = 3.0; // 3%


        protected CuentaAhorros() : base()
        {
        }

        public CuentaAhorros(string numeroCuenta, double saldoInicial, IEstadoCuenta estadoInicial)
            : base(numeroCuenta, saldoInicial, estadoInicial)
        {
        }

        public static CuentaAhorros Create(string numeroCuenta, double saldoInicial, IEstadoCuenta estadoInicial)
        {
            if (string.IsNullOrWhiteSpace(numeroCuenta)) throw new ArgumentException("Número de cuenta inválido.", nameof(numeroCuenta));
            if (estadoInicial == null) throw new ArgumentNullException(nameof(estadoInicial));

            return new CuentaAhorros(numeroCuenta, saldoInicial, estadoInicial);
        }

        public override void Retirar(double monto)
        {
            if (Saldo - monto < 0)
                throw new InvalidOperationException("Fondos insuficientes.");
            base.Retirar(monto);
        }

        public override void AplicarInteresMensual()
        {
            // Tratamos TASA_INTERES_AHORROS como porcentaje (ej. 3.0m => 3%)
            double tasaMensual = TASA_INTERES_AHORROS / 100.0 / 12.0;
            if (tasaMensual <= 0.0) return;

            double montoInteres = Saldo * tasaMensual;
            montoInteres = FinancialRounding.RoundMoney(montoInteres);
            if (montoInteres == 0.0) return;

            AplicarMontoInteres(montoInteres);
        }
    }
}
