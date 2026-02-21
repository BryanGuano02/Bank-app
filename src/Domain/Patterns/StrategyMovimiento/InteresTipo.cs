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

            // Determinar si es acreditación (cuenta de ahorros) o cobro (cuenta corriente en sobregiro)
            if (movimiento.Destino is CuentaAhorros)
            {
                // Acreditar interés a cuenta de ahorros
                movimiento.Destino.Depositar(movimiento.Monto);
            }
            else if (movimiento.Destino is CuentaCorriente cuentaCorriente)
            {
                // Cobrar interés de sobregiro a cuenta corriente
                movimiento.Destino.Retirar(movimiento.Monto);
            }
            else
            {
                throw new InvalidOperationException("El tipo de cuenta no soporta procesamiento de intereses.");
            }
        }

        public void validar(Movimiento movimiento)
        {
            if (movimiento == null) throw new ArgumentNullException(nameof(movimiento));

            if (movimiento.Destino == null)
                throw new InvalidOperationException("Cuenta destino inexistente para el procesamiento de intereses.");

            if (movimiento.Monto <= 0)
                throw new ArgumentOutOfRangeException(nameof(movimiento.Monto), "El monto de los intereses debe ser mayor que cero.");

            if (movimiento.Monto > MONTO_MAXIMO_POR_MOVIMIENTO)
                throw new InvalidOperationException($"El monto de interés excede el máximo permitido por movimiento ({MONTO_MAXIMO_POR_MOVIMIENTO}).");

            // Validar que sea un tipo de cuenta que soporte intereses
            if (!(movimiento.Destino is CuentaAhorros) && !(movimiento.Destino is CuentaCorriente))
            {
                throw new InvalidOperationException("Solo las cuentas de ahorro y corrientes pueden procesar intereses.");
            }
        }
    }
}
