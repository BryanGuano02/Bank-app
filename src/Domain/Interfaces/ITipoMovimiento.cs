using Domain.Entities;

namespace Domain.Interfaces.Types
{
    public interface ITipoMovimiento
    {
        void procesar(Movimiento movimiento);
        void validar(Movimiento movimiento);
    }
}
