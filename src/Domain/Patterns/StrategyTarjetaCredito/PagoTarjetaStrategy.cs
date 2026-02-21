using Domain.Entities;

namespace Fast_Bank.Domain.Patterns.StrategyTarjetaCredito;

public class PagoTarjetaStrategy : ITransaccionTarjetaStrategy
{
    public void Validar(TarjetaCredito tarjeta, double monto)
    {
        if (tarjeta == null)
            throw new ArgumentNullException(nameof(tarjeta), "La tarjeta de crédito no existe");

        if (monto <= 0)
            throw new ArgumentException("El monto del pago debe ser mayor a 0", nameof(monto));

        if (monto > tarjeta.SaldoUtilizado)
            throw new InvalidOperationException(
                $"El pago no puede ser mayor al saldo utilizado. Saldo actual: {tarjeta.SaldoUtilizado:C}");
    }

    public void Procesar(TarjetaCredito tarjeta, double monto)
    {
        tarjeta.ReducirDeuda(monto);
    }
}
