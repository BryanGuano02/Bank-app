using Domain.Interfaces.States;
using System;

namespace Domain.Entities
{
    public class TarjetaCredito
    {
        public string NumeroTarjeta { get; private set; }
        public decimal LimiteCredito { get; private set; }
        public decimal SaldoPendiente { get; private set; }
        public Cliente Cliente { get; private set; }

        private IEstadoTarjeta _estado;

        public TarjetaCredito(string numeroTarjeta, decimal limiteCredito, Cliente cliente, IEstadoTarjeta estadoInicial)
        {
            NumeroTarjeta = numeroTarjeta;
            LimiteCredito = limiteCredito;
            Cliente = cliente;
            _estado = estadoInicial;
            SaldoPendiente = 0;
        }

        public static TarjetaCredito Create(string numeroTarjeta, decimal limiteCredito, Cliente cliente, IEstadoTarjeta estadoInicial)
        {
            if (string.IsNullOrWhiteSpace(numeroTarjeta)) throw new ArgumentException("Número de tarjeta inválido.", nameof(numeroTarjeta));
            if (limiteCredito <= 0) throw new ArgumentOutOfRangeException(nameof(limiteCredito), "Límite de crédito debe ser mayor que cero.");
            if (cliente == null) throw new ArgumentNullException(nameof(cliente));
            if (estadoInicial == null) throw new ArgumentNullException(nameof(estadoInicial));

            return new TarjetaCredito(numeroTarjeta, limiteCredito, cliente, estadoInicial);
        }

        public void CambiarEstado(IEstadoTarjeta nuevoEstado)
        {
            _estado = nuevoEstado;
        }

        // Usado por el estado concreto para actualizar valores
        internal void AumentarDeuda(decimal monto) => SaldoPendiente += monto;
        internal void DisminuirDeuda(decimal monto) => SaldoPendiente -= monto;

        // --- Delegación ---
        public void RealizarCompra(decimal monto)
        {
            // Validaciones invariant del negocio antes del estado
            if (SaldoPendiente + monto > LimiteCredito)
                throw new InvalidOperationException("Límite excedido");

            // El estado decide si se permite la compra (ej: si está Bloqueada lanza error)
            _estado.RealizarCompra(this, monto);
        }
    }
}
