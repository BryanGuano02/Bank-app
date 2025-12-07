using System;
using Domain.Entities;
using Domain.Interfaces.Types;

namespace Domain.Services
{
    // Servicio de dominio que aplica una estrategia (pattern Strategy)
    // para validar y procesar un movimiento.
    public class Movimientoservice
    {
        public Movimientoservice() { }

        // Ejecuta el movimiento utilizando la estrategia proporcionada.
        // Lanza excepciones en caso de validación o procesamiento inválido.
        public void Ejecutar(Movimiento movimiento, ITipoMovimiento tipoMovimiento)
        {
            if (movimiento == null) throw new ArgumentNullException(nameof(movimiento));
            if (tipoMovimiento == null) throw new ArgumentNullException(nameof(tipoMovimiento));

            // Validación a través de la estrategia
            tipoMovimiento.validar(movimiento);

            // Procesamiento a través de la estrategia
            tipoMovimiento.procesar(movimiento);
        }
    }
}
