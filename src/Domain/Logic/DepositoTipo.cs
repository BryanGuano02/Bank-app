using System;
using Domain.Entities;
using Domain.Interfaces.Types;

namespace Domain.Logic
{
    public class DepositoTipo : ITipoMovimiento
    {
        private const decimal MONTO_MAXIMO_POR_MOVIMIENTO = 5000m;

        public void procesar(Movimiento movimiento)
        {
            validar(movimiento);
            movimiento.Destino.Depositar(movimiento.Monto);
        }

        public void validar(Movimiento movimiento)
        {
            if (movimiento == null) throw new ArgumentNullException(nameof(movimiento));

            if (movimiento.Destino == null)
                throw new InvalidOperationException("Cuenta destino inexistente para el depósito.");

            if (movimiento.Monto <= 0)
                throw new ArgumentOutOfRangeException(nameof(movimiento.Monto), "El monto debe ser mayor que cero.");

            if (movimiento.Monto > MONTO_MAXIMO_POR_MOVIMIENTO)
                throw new InvalidOperationException($"El monto excede el máximo permitido por movimiento ({MONTO_MAXIMO_POR_MOVIMIENTO}).");
        }
    }
}
