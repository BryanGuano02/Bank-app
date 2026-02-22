using System;
using Domain.Entities;
using Domain.Logic;
using Fast_Bank.Domain.Utils;

namespace Domain.Services
{
    public class InteresesService
    {
        public double CalcularInteresMensual(CuentaAhorros cuenta)
        {
            if (cuenta == null) throw new ArgumentNullException(nameof(cuenta));
            if (CuentaAhorros.TASA_INTERES_AHORROS < 0) throw new ArgumentOutOfRangeException(nameof(CuentaAhorros.TASA_INTERES_AHORROS), "La tasa de inter�s no puede ser negativa.");

            // F�rmula: Inter�s = Saldo * (TasaInteres / 12)
            // TasaInteres es anual en porcentaje (ej. 3.0 => 3%), por eso dividimos entre 12*100
            var interesMensual = cuenta.Saldo * (CuentaAhorros.TASA_INTERES_AHORROS / (12.0 * 100.0));

            return FinancialRounding.RoundMoney(interesMensual);
        }

        public double CalcularInteresSobregiro(CuentaCorriente cuenta)
        {
            if (cuenta == null) throw new ArgumentNullException(nameof(cuenta));

            // Solo calcular inter�s si el saldo es negativo (est� en sobregiro)
            if (cuenta.Saldo >= 0)
                return 0;

            // Calcular inter�s sobre el monto en sobregiro (valor absoluto del saldo negativo)
            // F�rmula: Inter�s mensual = |Saldo| * (InteresSobregiro / 12)
            // InteresSobregiro se almacena como tasa anual en formato decimal (ej. 0.22 => 22%)
            var montoSobregiro = Math.Abs(cuenta.Saldo);
            var tasaMensual = cuenta.InteresSobregiro / 12.0;
            var interesSobregiro = montoSobregiro * tasaMensual;

            return FinancialRounding.RoundMoney(interesSobregiro);
        }

    }
}
