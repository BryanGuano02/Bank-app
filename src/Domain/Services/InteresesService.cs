using System;
using Domain.Entities;
using Domain.Logic;
using Fast_Bank.Domain.Utils;

namespace Domain.Services
{
    public class InteresesService
    {
        public decimal CalcularInteresMensual(CuentaAhorros cuenta)
        {
            if (cuenta == null) throw new ArgumentNullException(nameof(cuenta));
            if (cuenta.TasaInteres < 0) throw new ArgumentOutOfRangeException(nameof(cuenta.TasaInteres), "La tasa de inter�s no puede ser negativa.");

            // F�rmula: Inter�s = Saldo * (TasaInteres / 12)
            // TasaInteres es anual, dividimos entre 12 para obtener la mensual
            var interesMensual = cuenta.Saldo * (decimal)(cuenta.TasaInteres / (12 * 100));

            return FinancialRounding.RoundMoney(interesMensual);
        }

        public Movimiento CrearYEjecutarAcreditacionInteres(string idMovimiento, CuentaAhorros cuenta, decimal montoInteres)
        {
            if (cuenta == null) throw new ArgumentNullException(nameof(cuenta));
            if (string.IsNullOrWhiteSpace(idMovimiento)) throw new ArgumentException("IdMovimiento inv�lido.", nameof(idMovimiento));
            if (montoInteres <= 0) throw new ArgumentOutOfRangeException(nameof(montoInteres), "El monto de inter�s debe ser mayor que cero.");

            var descripcion = $"Inter�s mensual - Tasa: {cuenta.TasaInteres:P2}";

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

        public decimal CalcularInteresSobregiro(CuentaCorriente cuenta)
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

        public Movimiento CrearYEjecutarCargoInteresSobregiro(string idMovimiento, CuentaCorriente cuenta, decimal montoInteres)
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
