using Domain.Entities;

namespace Domain.Interfaces.States
{
    public interface IEstadoTarjeta
    {
        void RealizarCompra(TarjetaCredito tarjeta, decimal monto);
        void PagarTarjeta(TarjetaCredito tarjeta, decimal monto);
    }
}
