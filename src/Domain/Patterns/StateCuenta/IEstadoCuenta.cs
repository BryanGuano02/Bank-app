using Domain.Entities;

namespace Domain.Interfaces.States
{

    public interface IEstadoCuenta
    {
        string Nombre { get; }
        void Depositar(Cuenta cuenta, double monto);
        void Retirar(Cuenta cuenta, double monto);
    }
}
