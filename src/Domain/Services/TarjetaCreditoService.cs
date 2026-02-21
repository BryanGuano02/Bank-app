using Domain.Entities;
using Fast_Bank.Domain.Patterns.StrategyTarjetaCredito;

namespace Domain.Services;

public class TarjetaCreditoService
{
    public void EjecutarOperacion(
        ITransaccionTarjetaStrategy estrategia,
        TarjetaCredito tarjeta,
        double monto,
        string descripcion)
    {
        if (estrategia == null)
            throw new ArgumentNullException(nameof(estrategia), "La estrategia no puede ser nula");

        if (tarjeta == null)
            throw new ArgumentNullException(nameof(tarjeta), "La tarjeta de crédito no puede ser nula");

        // Validar la operación
        estrategia.Validar(tarjeta, monto);

        // Procesar la operación
        estrategia.Procesar(tarjeta, monto);
    }
}
