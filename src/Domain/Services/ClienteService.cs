using Domain.Entities;

namespace Domain.Services
{
    public class ClienteService
    {

        public ClienteService()
        {

        }

        public Cliente CrearCliente(string cedula, string nombre, string apellido, string direccion, string correo, string telefono)
        {
            return Cliente.Create(cedula, nombre, apellido, direccion, correo, telefono);
        }

        public Cliente ActualizarCliente(Cliente cliente, string nombre, string apellido, string direccion, string correo, string telefono)
        {
            if (cliente == null) throw new ArgumentNullException(nameof(cliente));

            cliente.Update(nombre, apellido, direccion, correo, telefono);

            return cliente;
        }

        public CuentaCorriente CrearCuentaCorriente(Cliente cliente, string numeroCuenta, decimal saldoInicial, decimal limiteSobregiro, Interfaces.States.IEstadoCuenta? estadoInicial = null)
        {
            if (cliente == null) throw new ArgumentNullException(nameof(cliente));
            if (string.IsNullOrWhiteSpace(numeroCuenta)) throw new ArgumentException("Número de cuenta inválido.", nameof(numeroCuenta));
            if (limiteSobregiro < 0) throw new ArgumentOutOfRangeException(nameof(limiteSobregiro), "Límite de sobregiro no puede ser negativo.");

            var estado = estadoInicial ?? new Domain.Patterns.State.EstadoCuentaActiva();

            var cuenta = CuentaCorriente.Create(numeroCuenta, saldoInicial, limiteSobregiro, estado);

            cuenta.SetCliente(cliente);
            cliente.SetCuenta(cuenta);

            return cuenta;
        }

        public CuentaAhorros CrearCuentaAhorros(Cliente cliente, string numeroCuenta, decimal saldoInicial, double tasaInteres, Interfaces.States.IEstadoCuenta? estadoInicial = null)
        {
            if (cliente == null) throw new ArgumentNullException(nameof(cliente));
            if (string.IsNullOrWhiteSpace(numeroCuenta)) throw new ArgumentException("Número de cuenta inválido.", nameof(numeroCuenta));
            if (tasaInteres < 0) throw new ArgumentOutOfRangeException(nameof(tasaInteres), "Tasa de interés no puede ser negativa.");

            var estado = estadoInicial ?? new Domain.Patterns.State.EstadoCuentaActiva();

            var cuenta = CuentaAhorros.Create(numeroCuenta, saldoInicial, tasaInteres, estado);

            // Associate both sides of the relationship
            cuenta.SetCliente(cliente);
            cliente.SetCuenta(cuenta);

            return cuenta;
        }

    }
}
