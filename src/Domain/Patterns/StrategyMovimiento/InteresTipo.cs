using System;
using Domain.Entities;
using Domain.Interfaces.Types;

namespace Domain.Logic
{
    public class InteresTipo : ITipoMovimiento
    {
        private const decimal MONTO_MAXIMO_POR_MOVIMIENTO = 5000m;

        public void procesar(Movimiento movimiento)
        {
            validar(movimiento);

            // Acredita el monto del movimiento en la cuenta destino (cuenta de ahorros).
            movimiento.Destino.Depositar(movimiento.Monto);
        }

        public void validar(Movimiento movimiento)
        {
            if (movimiento == null) throw new ArgumentNullException(nameof(movimiento));

            if (movimiento.Destino == null)
                throw new InvalidOperationException("Cuenta destino inexistente para la acreditación de intereses.");

            // Sólo cuentas de ahorro deben recibir intereses
            if (!(movimiento.Destino is CuentaAhorros))
                throw new InvalidOperationException("Sólo las cuentas de ahorro pueden recibir intereses.");

            if (movimiento.Monto <= 0)
                throw new ArgumentOutOfRangeException(nameof(movimiento.Monto), "El monto de los intereses debe ser mayor que cero.");

            if (movimiento.Monto > MONTO_MAXIMO_POR_MOVIMIENTO)
                throw new InvalidOperationException($"El monto de interés excede el máximo permitido por movimiento ({MONTO_MAXIMO_POR_MOVIMIENTO}).");
        }
    }
}
