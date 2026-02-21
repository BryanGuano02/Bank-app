using Domain.Entities;

namespace Fast_Bank.Domain.Patterns.StrategyTarjetaCredito;

public class CompraTarjetaStrategy : ITransaccionTarjetaStrategy
{
    public void Validar(TarjetaCredito tarjeta, double monto)
    {
        if (tarjeta == null)
            throw new ArgumentNullException(nameof(tarjeta), "La tarjeta de crédito no existe");

        if (monto <= 0)
            throw new ArgumentException("El monto de la compra debe ser mayor a 0", nameof(monto));

        if (tarjeta.EstaVencida())
            throw new InvalidOperationException("No se puede realizar la compra. La tarjeta está vencida");

        if (tarjeta.CreditoDisponible < monto)
            throw new InvalidOperationException(
                $"Crédito insuficiente. Disponible: {tarjeta.CreditoDisponible:C}, Requerido: {monto:C}");
    }

    public void Procesar(TarjetaCredito tarjeta, double monto)
    {
        tarjeta.IncrementarDeuda(monto);
    }
}
