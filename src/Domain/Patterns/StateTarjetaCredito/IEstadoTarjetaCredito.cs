using Domain.Entities;

namespace Domain.Interfaces.States
{
    public interface IEstadoTarjeta
    {
        void RealizarCompra(TarjetaCredito tarjeta, double monto);
        void PagarTarjeta(TarjetaCredito tarjeta, double monto);
    }
}
