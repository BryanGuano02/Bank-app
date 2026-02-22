
using Domain.Interfaces.States;
using Domain.Logic;
using Domain.ValueObjects;
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

        public DetalleAcreditacion? AplicarInteresMensual(double montoInteres)
        {
            if (montoInteres <= 0)
                return null;

            var saldoAnterior = this.Saldo;

            // 1. La entidad modifica su propio estado
            ModificarSaldo(montoInteres);

            // 2. La entidad crea su propio Movimiento (garantiza consistencia del Agregado)
            var movimiento = Movimiento.Create(
                Guid.NewGuid().ToString(),
                montoInteres,
                null,
                this,
                $"Interés mensual - Tasa: {TASA_INTERES_AHORROS:P2}",
                new InteresTipo()
            );

            _movimientos.Add(movimiento);

            // 3. Retorna la información necesaria para el reporte (Application layer)
            return new DetalleAcreditacion
            {
                NumeroCuenta = this.NumeroCuenta,
                SaldoAnterior = saldoAnterior,
                MontoInteres = montoInteres,
                SaldoNuevo = this.Saldo,
                TasaAplicada = TASA_INTERES_AHORROS
            };
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
