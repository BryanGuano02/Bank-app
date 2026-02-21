using Domain.Entities;

namespace Fast_Bank.Domain.Patterns.StrategyTarjetaCredito;

public interface ITransaccionTarjetaStrategy
{
    void Validar(TarjetaCredito tarjeta, double monto);
    void Procesar(TarjetaCredito tarjeta, double monto);
}
