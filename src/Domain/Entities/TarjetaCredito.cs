using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class TarjetaCredito
    {
        [Key]
        public string NumeroTarjeta { get; private set; }
        public double LimiteCredito { get; private set; }
        public double SaldoUtilizado { get; private set; }
        public DateTime FechaEmision { get; private set; }
        public DateTime FechaVencimiento { get; private set; }
        public double TasaInteresMensual { get; private set; }
        public double CreditoDisponible { get; private set; }
        public double PagoMinimo { get; private set; }

        public string IdCliente { get; set; } = string.Empty;
        public Cliente? Cliente { get; set; }

        // Constructor parameterless para EF Core
        protected TarjetaCredito()
        {
            NumeroTarjeta = string.Empty;
        }

        public TarjetaCredito(string numeroTarjeta, double limiteCredito, DateTime fechaEmision, DateTime fechaVencimiento, double tasaInteresMensual)
        {
            NumeroTarjeta = numeroTarjeta;
            LimiteCredito = limiteCredito;
            FechaEmision = fechaEmision;
            FechaVencimiento = fechaVencimiento;
            TasaInteresMensual = tasaInteresMensual;
            SaldoUtilizado = 0;
            CreditoDisponible = limiteCredito;
            PagoMinimo = 0;
        }

        public static TarjetaCredito Create(string numeroTarjeta, double limiteCredito, DateTime fechaEmision, DateTime fechaVencimiento, double tasaInteresMensual)
        {
            if (string.IsNullOrWhiteSpace(numeroTarjeta)) throw new ArgumentException("Número de tarjeta inválido.", nameof(numeroTarjeta));
            if (limiteCredito <= 0) throw new ArgumentOutOfRangeException(nameof(limiteCredito), "Límite de crédito debe ser mayor que cero.");
            if (fechaVencimiento <= fechaEmision) throw new ArgumentException("La fecha de vencimiento debe ser posterior a la fecha de emisión.");
            if (tasaInteresMensual < 0) throw new ArgumentOutOfRangeException(nameof(tasaInteresMensual), "La tasa de interés no puede ser negativa.");

            return new TarjetaCredito(numeroTarjeta, limiteCredito, fechaEmision, fechaVencimiento, tasaInteresMensual);
        }

        internal void IncrementarDeuda(double monto)
        {
            SaldoUtilizado += monto;
            CreditoDisponible = LimiteCredito - SaldoUtilizado;
            CalcularPagoMinimo();
        }

        internal void ReducirDeuda(double monto)
        {
            SaldoUtilizado -= monto;
            CreditoDisponible = LimiteCredito - SaldoUtilizado;
            CalcularPagoMinimo();
        }

        private void CalcularPagoMinimo()
        {
            // Normalmente el pago mínimo es un porcentaje del saldo (ej. 5% o un mínimo fijo)
            PagoMinimo = SaldoUtilizado > 0 ? Math.Max(SaldoUtilizado * 0.05, 10) : 0;
        }

        public bool EstaVencida()
        {
            return DateTime.Now > FechaVencimiento;
        }
    }
}
