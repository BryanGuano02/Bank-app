using System;
using Domain.Entities;
using Domain.Logic;

namespace Domain.Services
{
    public class MovimientoService
    {
        public MovimientoService()
        {
        }

        // Crea y ejecuta la lógica de un depósito. No persiste ni accede a la BD.
        public Movimiento CrearYEjecutarDeposito(string idMovimiento, Cuenta destino, decimal monto, string descripcion)
        {
            if (destino == null) throw new ArgumentNullException(nameof(destino));
            if (string.IsNullOrWhiteSpace(idMovimiento)) throw new ArgumentException("IdMovimiento inválido.", nameof(idMovimiento));

            var movimiento = Movimiento.Create(idMovimiento, monto, null, destino, descripcion ?? string.Empty, new DepositoTipo());

            // Ejecuta la estrategia (modifica la entidad Cuenta en memoria)
            movimiento.Ejecutar();

            return movimiento;
        }

        // Crea y ejecuta la lógica de un retiro. No persiste ni accede a la BD.
        public Movimiento CrearYEjecutarRetiro(string idMovimiento, Cuenta origen, decimal monto, string descripcion)
        {
            if (origen == null) throw new ArgumentNullException(nameof(origen));
            if (string.IsNullOrWhiteSpace(idMovimiento)) throw new ArgumentException("IdMovimiento inválido.", nameof(idMovimiento));

            var movimiento = Movimiento.Create(idMovimiento, monto, origen, origen, descripcion ?? string.Empty, new RetiroTipo());

            // Ejecuta la estrategia (modifica la entidad Cuenta en memoria)
            // La validación de saldo y límites se hace en:
            // 1. RetiroTipo.validar() - valida monto máximo (5000)
            // 2. Cuenta.Retirar() - delega al estado
            // 3. CuentaCorriente.Retirar() o CuentaAhorros.Retirar() - valida sobregiro/saldo
            movimiento.Ejecutar();

            return movimiento;
        }
    }
}
