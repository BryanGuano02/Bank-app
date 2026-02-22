
using Domain.Interfaces.States;
using Domain.Logic;
using Domain.ValueObjects;
using Fast_Bank.Domain.Utils;
using System;

namespace Domain.Entities
{
    public class CuentaCorriente : Cuenta
    {
        public const double TASA_INTERES_SOBREGIRO = 0.22; // 22%
        public double LimiteSobregiro { get; private set; } = 200.0;
        public double InteresSobregiro { get; private set; } = TASA_INTERES_SOBREGIRO;

        // Parameterless constructor for EF Core
        protected CuentaCorriente() : base()
        {
        }

        public CuentaCorriente(string numeroCuenta, double saldoInicial, IEstadoCuenta estadoInicial)
            : base(numeroCuenta, saldoInicial, estadoInicial)
        {
        }

        public static CuentaCorriente Create(string numeroCuenta, double saldoInicial, IEstadoCuenta estadoInicial)
        {
            if (string.IsNullOrWhiteSpace(numeroCuenta)) throw new ArgumentException("Número de cuenta inválido.", nameof(numeroCuenta));
            if (estadoInicial == null) throw new ArgumentNullException(nameof(estadoInicial));

            return new CuentaCorriente(numeroCuenta, saldoInicial, estadoInicial);
        }

        public override void Retirar(double monto)
        {
            if ((Saldo + LimiteSobregiro) - monto < 0)
                throw new InvalidOperationException("Excede límite de sobregiro.");

            base.Retirar(monto);
        }

        /// <summary>
        /// Aplica un cargo de interés por sobregiro pre-calculado por el Domain Service.
        /// Modifica el propio estado, crea el Movimiento y lo añade al Agregado.
        /// </summary>
        /// <returns>Detalle del cobro, o null si el monto no aplica.</returns>
        public DetalleCobro? AplicarInteresSobregiro(double montoInteres)
        {
            if (montoInteres <= 0)
                return null;

            var saldoAnterior = this.Saldo;

            // 1. La entidad modifica su propio estado (resta, incrementa el sobregiro)
            ModificarSaldo(-montoInteres);

            // 2. La entidad crea su propio Movimiento (garantiza consistencia del Agregado)
            var movimiento = Movimiento.Create(
                Guid.NewGuid().ToString(),
                montoInteres,
                null,
                this,
                $"Interés por sobregiro - Tasa: {InteresSobregiro:P2}",
                new InteresTipo()
            );

            _movimientos.Add(movimiento);

            // 3. Retorna la información necesaria para el reporte (Application layer)
            return new DetalleCobro
            {
                NumeroCuenta = this.NumeroCuenta,
                SaldoAnterior = saldoAnterior,
                MontoInteres = montoInteres,
                SaldoNuevo = this.Saldo,
                TasaAplicada = InteresSobregiro
            };
        }

        /// <summary>
        /// Sobrescritura del método base (sin parámetros) para compatibilidad interna.
        /// </summary>
        public override void AplicarInteresMensual()
        {
            // Interés de sobregiro: InteresSobregiro está en formato double (ej. 0.22 => 22%)
            double tasaMensual = InteresSobregiro / 12.0;
            if (tasaMensual <= 0.0) return;

            // Aplicar interés sólo si hay saldo negativo (se cobra en sobregiro)
            if (Saldo < 0.0)
            {
                double montoSobregiro = Math.Abs(Saldo);
                double montoInteres = montoSobregiro * tasaMensual;
                montoInteres = FinancialRounding.RoundMoney(montoInteres);
                if (montoInteres == 0.0) return;

                // Cargar el interés (resta al saldo)
                AplicarMontoInteres(-montoInteres);
            }
        }
    }
}
