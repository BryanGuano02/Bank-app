using System;
using Domain.Entities;
using Domain.Logic;

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
            var interesMensual = cuenta.Saldo * (decimal)(cuenta.TasaInteres / (12*100));

            return Math.Round(interesMensual, 2); // Redondear a 2 decimales
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
    }
}