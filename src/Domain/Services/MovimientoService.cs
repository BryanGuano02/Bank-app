using System;
using Domain.Entities;
using Domain.Logic;

namespace Domain.Services
{
    public class MovimientoService
    {
        public const double MONTO_MAXIMO_TRANSACCION = 5000.0;

        public MovimientoService()
        {
        }

        private static void ValidarMonto(double monto)
        {
            if (monto <= 0)
                throw new ArgumentOutOfRangeException(nameof(monto), "El monto debe ser mayor que cero.");
            if (monto > MONTO_MAXIMO_TRANSACCION)
                throw new InvalidOperationException($"El monto no puede superar el límite de {MONTO_MAXIMO_TRANSACCION:N2} por transacción.");
        }

        // Crea y ejecuta la lógica de un depósito. No persiste ni accede a la BD.
        public Movimiento Depositar(string idMovimiento, Cuenta destino, double monto, string descripcion)
        {
            if (string.IsNullOrWhiteSpace(idMovimiento)) throw new ArgumentException("IdMovimiento inválido.", nameof(idMovimiento));
            if (destino == null) throw new ArgumentNullException(nameof(destino));
            ValidarMonto(monto);

            var movimiento = Movimiento.Create(idMovimiento, monto, null, destino, descripcion ?? string.Empty, new DepositoTipo());

            // Ejecuta la estrategia (modifica la entidad Cuenta en memoria)
            movimiento.Ejecutar();

            return movimiento;
        }

        // Crea y ejecuta la lógica de un retiro. No persiste ni accede a la BD.
        public Movimiento Retirar(string idMovimiento, Cuenta origen, double monto, string descripcion)
        {
            if (string.IsNullOrWhiteSpace(idMovimiento)) throw new ArgumentException("IdMovimiento inválido.", nameof(idMovimiento));
            if (origen == null) throw new ArgumentNullException(nameof(origen));
            ValidarMonto(monto);

            var movimiento = Movimiento.Create(idMovimiento, monto, origen, null, descripcion ?? string.Empty, new RetiroTipo());

            movimiento.Ejecutar();

            return movimiento;
        }

        // Crea y ejecuta la lógica de una transferencia. No persiste ni accede a la BD.
        public Movimiento Transferir(string idMovimiento, Cuenta origen, Cuenta destino, double monto, string descripcion)
        {
            if (string.IsNullOrWhiteSpace(idMovimiento)) throw new ArgumentException("IdMovimiento inválido.", nameof(idMovimiento));
            if (origen == null) throw new ArgumentNullException(nameof(origen));
            if (destino == null) throw new ArgumentNullException(nameof(destino));
            ValidarMonto(monto);

            var movimiento = Movimiento.Create(idMovimiento, monto, origen, destino, descripcion ?? string.Empty, new TransferenciaTipo());

            movimiento.Ejecutar();

            return movimiento;
        }
    }
}
