using Domain.Entities;

namespace Domain.Interfaces.States
{
    public interface IEstadoCuenta
    {
        void Depositar(Cuenta cuenta, double monto);
        void Retirar(Cuenta cuenta, double monto);
    }
}
