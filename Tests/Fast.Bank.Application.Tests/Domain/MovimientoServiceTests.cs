using System;
using Domain.Entities;
using Domain.Services;
using Xunit;
using Domain.Patterns.State;

namespace Fast.Bank.Application.Tests.Domain
{
    public class MovimientoServiceTests
    {
        private readonly MovimientoService _movimientoService;

        public MovimientoServiceTests()
        {
            _movimientoService = new MovimientoService();
        }



        [Fact]
        public void CrearYEjecutarDeposito_ConDatosValidos_DebeAumentarSaldoDeCuentaDestino()
        {
            // Arrange
            var saldoInicial = 100m;
            var montoDeposito = 50m;
            var cuentaDestino = new CuentaAhorros("12345", saldoInicial, 0.01, new EstadoCuentaActiva());
            var idMovimiento = Guid.NewGuid().ToString();
            var descripcion = "Depósito de prueba";

            // Act
            var movimiento = _movimientoService.CrearYEjecutarDeposito(idMovimiento, cuentaDestino, montoDeposito, descripcion);

            // Assert
            Assert.NotNull(movimiento);
            Assert.Equal(idMovimiento, movimiento.IdMovimiento);
            Assert.Equal(montoDeposito, movimiento.Monto);
            Assert.Equal(saldoInicial + montoDeposito, cuentaDestino.Saldo); // El saldo debe haberse actualizado
        }

        [Fact]
        public void CrearYEjecutarDeposito_ConCuentaDestinoNula_DebeLanzarArgumentNullException()
        {
            // Arrange
            var montoDeposito = 50m;
            var idMovimiento = Guid.NewGuid().ToString();
            var descripcion = "Depósito de prueba";

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                _movimientoService.CrearYEjecutarDeposito(idMovimiento, null, montoDeposito, descripcion)
            );
            Assert.Equal("destino", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void CrearYEjecutarDeposito_ConIdMovimientoInvalido_DebeLanzarArgumentException(string idMovimiento)
        {
            // Arrange
            var cuentaDestino = new CuentaAhorros("12345", 100m, 0.01, new EstadoCuentaActiva());
            var montoDeposito = 50m;
            var descripcion = "Depósito de prueba";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                _movimientoService.CrearYEjecutarDeposito(idMovimiento, cuentaDestino, montoDeposito, descripcion)
            );
            Assert.Equal("IdMovimiento inválido. (Parameter 'idMovimiento')", exception.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-50)]
        public void CrearYEjecutarDeposito_ConMontoInvalido_DebeLanzarArgumentOutOfRangeException(decimal monto)
        {
            // Arrange
            var cuentaDestino = new CuentaAhorros("12345", 100m, 0.01, new EstadoCuentaActiva());
            var idMovimiento = Guid.NewGuid().ToString();
            var descripcion = "Depósito de prueba";

            // Act & Assert
            // La validación está en Movimiento.Create
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                _movimientoService.CrearYEjecutarDeposito(idMovimiento, cuentaDestino, monto, descripcion)
            );
            Assert.Equal("monto", exception.ParamName);
        }

        [Fact]
        public void CrearYEjecutarDeposito_ConMontoMayorAlPermitido_DebeLanzarInvalidOperationException()
        {
            // Arrange
            var cuentaDestino = new CuentaAhorros("12345", 100m, 0.01, new EstadoCuentaActiva());
            var montoExcesivo = 5001m; // El límite es 5000
            var idMovimiento = Guid.NewGuid().ToString();
            var descripcion = "Depósito de prueba";

            // Act & Assert
            // La validación está en la estrategia DepositoTipo
            var exception = Assert.Throws<InvalidOperationException>(() =>
                _movimientoService.CrearYEjecutarDeposito(idMovimiento, cuentaDestino, montoExcesivo, descripcion)
            );
            Assert.Contains("El monto excede el máximo permitido por movimiento", exception.Message);
        }
    }
}
