using Domain.Entities;
using Fast_Bank.Domain.Patterns.StrategyTarjetaCredito;

namespace Domain.Services;

public class TarjetaCreditoService
{
    private const int NUMERO_TARJETA_LEN = 16;
    private const double LIMITE_CREDITO_DEFAULT = 8000.00;
    private const double TASA_INTERES_MENSUAL_DEFAULT = 3.50;

    private static int _ultimoNumero = 0;
    private static readonly object _lock = new object();

    private string GenerarProximoNumeroTarjeta()
    {
        int numeroActual;
        lock (_lock)
        {
            _ultimoNumero++;
            numeroActual = _ultimoNumero;
        }

        string numStr = numeroActual.ToString();

        if (numStr.Length <= NUMERO_TARJETA_LEN)
        {
            return numStr.PadLeft(NUMERO_TARJETA_LEN, '0');
        }
        else
        {
            return numStr.Substring(numStr.Length - NUMERO_TARJETA_LEN);
        }
    }

    public TarjetaCredito CrearTarjetaCredito(string idCliente)
    {
        if (string.IsNullOrWhiteSpace(idCliente))
            throw new ArgumentNullException(nameof(idCliente), "El ID del cliente no puede ser nulo o vacío");

        var numeroTarjeta = GenerarProximoNumeroTarjeta();

        var tarjeta = TarjetaCredito.Create(
            numeroTarjeta,
            LIMITE_CREDITO_DEFAULT,
            DateTime.Now,
            DateTime.Now.AddYears(3),
            TASA_INTERES_MENSUAL_DEFAULT);

        tarjeta.IdCliente = idCliente;

        return tarjeta;
    }

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
