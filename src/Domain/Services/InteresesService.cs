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

        public Movimiento CrearYEjecutarAcreditacionInteres(string idMovimiento, CuentaAhorros cuenta, double montoInteres)
        {
            if (cuenta == null) throw new ArgumentNullException(nameof(cuenta));
            if (string.IsNullOrWhiteSpace(idMovimiento)) throw new ArgumentException("IdMovimiento inv�lido.", nameof(idMovimiento));
            if (montoInteres <= 0) throw new ArgumentOutOfRangeException(nameof(montoInteres), "El monto de inter�s debe ser mayor que cero.");

            var descripcion = $"Inter�s mensual - Tasa: {CuentaAhorros.TASA_INTERES_AHORROS:P2}";

            var movimiento = Movimiento.Create(
                idMovimiento,
                montoInteres,
                null, // Sin cuenta origen (es un proceso autom�tico)
                cuenta,
                descripcion,
                new InteresTipo()
            );

            // Ejecutar la estrategia (deposita el inter�s)
            movimiento.Ejecutar();

            return movimiento;
        }

        public double CalcularInteresSobregiro(CuentaCorriente cuenta)
        {
            if (cuenta == null) throw new ArgumentNullException(nameof(cuenta));

            // Solo calcular inter�s si el saldo es negativo (est� en sobregiro)
            if (cuenta.Saldo >= 0)
                return 0;

            // Calcular inter�s sobre el monto en sobregiro (valor absoluto del saldo negativo)
            // F�rmula: Inter�s = |Saldo| * InteresSobregiro
            var montoSobregiro = Math.Abs(cuenta.Saldo);
            var interesSobregiro = montoSobregiro * cuenta.InteresSobregiro;

            return FinancialRounding.RoundMoney(interesSobregiro);
        }

        public Movimiento CrearYEjecutarCargoInteresSobregiro(string idMovimiento, CuentaCorriente cuenta, double montoInteres)
        {
            if (cuenta == null) throw new ArgumentNullException(nameof(cuenta));
            if (string.IsNullOrWhiteSpace(idMovimiento)) throw new ArgumentException("IdMovimiento inv�lido.", nameof(idMovimiento));
            if (montoInteres <= 0) throw new ArgumentOutOfRangeException(nameof(montoInteres), "El monto de inter�s debe ser mayor que cero.");

            var descripcion = $"Inter�s por sobregiro - Tasa: {cuenta.InteresSobregiro:P2}";

            // Crear movimiento de cargo para intereses de sobregiro
            var movimiento = Movimiento.Create(
                idMovimiento,
                montoInteres,
                null,   // Sin cuenta origen (es un cargo autom�tico)
                cuenta, // La cuenta corriente es el destino (se le cobra)
                descripcion,
                new InteresTipo() // Usar la misma estrategia InteresTipo para todos los intereses
            );

            // Ejecutar la estrategia (retira/cobra el inter�s)
            movimiento.Ejecutar();

            return movimiento;
        }
    }
}
