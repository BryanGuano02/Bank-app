using Domain.Entities;

namespace Domain.Services
{
    public class CuentaService
    {
        private const int NUMERO_CUENTA_LEN = 10;
        private static int _ultimoNumero = 0;
        private static readonly object _lock = new object();

        public CuentaService()
        {
        }

        private string GenerarProximoNumeroCuenta()
        {
            int numeroActual;
            lock (_lock)
            {
                _ultimoNumero++;
                numeroActual = _ultimoNumero;
            }

            string numStr = numeroActual.ToString();

            if (numStr.Length <= NUMERO_CUENTA_LEN)
            {
                return numStr.PadLeft(NUMERO_CUENTA_LEN, '0');
            }
            else
            {
                return numStr.Substring(numStr.Length - NUMERO_CUENTA_LEN);
            }
        }

        public CuentaCorriente CrearCuentaCorriente(Cliente cliente, decimal saldoInicial)
        {
            if (cliente == null) throw new ArgumentNullException(nameof(cliente));

            var numeroCuenta = GenerarProximoNumeroCuenta();
            var estado = new Patterns.State.EstadoCuentaActiva();

            var cuenta = CuentaCorriente.Create(numeroCuenta, saldoInicial, estado);

            cuenta.SetCliente(cliente);
            cliente.SetCuenta(cuenta);

            return cuenta;
        }

        public CuentaAhorros CrearCuentaAhorros(Cliente cliente, decimal saldoInicial)
        {
            if (cliente == null) throw new ArgumentNullException(nameof(cliente));

            var numeroCuenta = GenerarProximoNumeroCuenta();
            var estado = new Patterns.State.EstadoCuentaActiva();

            var cuenta = CuentaAhorros.Create(numeroCuenta, saldoInicial, estado);

            cuenta.SetCliente(cliente);
            cliente.SetCuenta(cuenta);

            return cuenta;
        }
    }
}
