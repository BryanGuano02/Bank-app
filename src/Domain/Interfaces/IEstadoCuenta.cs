using Domain.Entities;

namespace Domain.Interfaces.States
{
    public interface IEstadoCuenta
    {
        void Depositar(Cuenta cuenta, decimal monto);
        void Retirar(Cuenta cuenta, decimal monto);
    }
}
