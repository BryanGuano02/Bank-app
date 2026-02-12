using Domain.Entities;

namespace Domain.Services
{
    public class CuentaService
    {
        public CuentaService()
        {
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

            cuenta.SetCliente(cliente);
            cliente.SetCuenta(cuenta);

            return cuenta;
        }
    }
}
