using Domain.Entities;

namespace Domain.Services
{
    public class ClienteService
    {
        private readonly CuentaService _cuentaService;

        public ClienteService(CuentaService cuentaService)
        {
            _cuentaService = cuentaService ?? throw new ArgumentNullException(nameof(cuentaService));
        }

        public Cliente CrearClienteConCuentaCorriente(
            string cedula, string nombre, string apellido, string direccion, string correo, string telefono,
            decimal saldoInicial)
        {
            var cliente = Cliente.Create(cedula, nombre, apellido, direccion, correo, telefono);
            _cuentaService.CrearCuentaCorriente(cliente, saldoInicial);
            return cliente;
        }

        public Cliente CrearClienteConCuentaAhorros(
            string cedula, string nombre, string apellido, string direccion, string correo, string telefono,
            decimal saldoInicial, double tasaInteres)
        {
            var cliente = Cliente.Create(cedula, nombre, apellido, direccion, correo, telefono);
            _cuentaService.CrearCuentaAhorros(cliente, saldoInicial, tasaInteres);
            return cliente;
        }

        public Cliente ActualizarCliente(Cliente cliente, string nombre, string apellido, string direccion, string correo, string telefono)
        {
            if (cliente == null) throw new ArgumentNullException(nameof(cliente));

            cliente.Update(nombre, apellido, direccion, correo, telefono);

            return cliente;
        }
    }
}
