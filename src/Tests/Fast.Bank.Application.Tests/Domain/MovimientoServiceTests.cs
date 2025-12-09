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

        #region Tests de Depósito

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

        #endregion

        #region Tests de Retiro

        [Fact]
        public void CrearYEjecutarRetiro_ConDatosValidos_DebeDisminuirSaldoDeCuentaOrigen()
        {
            // Arrange
            var saldoInicial = 1000m;
            var montoRetiro = 300m;
            var cuentaOrigen = new CuentaAhorros("12345", saldoInicial, 0.01, new EstadoCuentaActiva());
            var idMovimiento = Guid.NewGuid().ToString();
            var descripcion = "Retiro de prueba";

            // Act
            var movimiento = _movimientoService.CrearYEjecutarRetiro(idMovimiento, cuentaOrigen, montoRetiro, descripcion);

            // Assert
            Assert.NotNull(movimiento);
            Assert.Equal(idMovimiento, movimiento.IdMovimiento);
            Assert.Equal(montoRetiro, movimiento.Monto);
            Assert.Equal(saldoInicial - montoRetiro, cuentaOrigen.Saldo); // El saldo debe haberse reducido
        }

        [Fact]
        public void CrearYEjecutarRetiro_ConCuentaOrigenNula_DebeLanzarArgumentNullException()
        {
            // Arrange
            var montoRetiro = 100m;
            var idMovimiento = Guid.NewGuid().ToString();
            var descripcion = "Retiro de prueba";

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                _movimientoService.CrearYEjecutarRetiro(idMovimiento, null, montoRetiro, descripcion)
            );
            Assert.Equal("origen", exception.ParamName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CrearYEjecutarRetiro_ConIdMovimientoInvalido_DebeLanzarArgumentException(string idMovimiento)
        {
            // Arrange
            var cuentaOrigen = new CuentaAhorros("12345", 1000m, 0.01, new EstadoCuentaActiva());
            var montoRetiro = 100m;
            var descripcion = "Retiro de prueba";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                _movimientoService.CrearYEjecutarRetiro(idMovimiento, cuentaOrigen, montoRetiro, descripcion)
            );
            Assert.Equal("IdMovimiento inválido. (Parameter 'idMovimiento')", exception.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        public void CrearYEjecutarRetiro_ConMontoInvalido_DebeLanzarArgumentOutOfRangeException(decimal monto)
        {
            // Arrange
            var cuentaOrigen = new CuentaAhorros("12345", 1000m, 0.01, new EstadoCuentaActiva());
            var idMovimiento = Guid.NewGuid().ToString();
            var descripcion = "Retiro de prueba";

            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
                _movimientoService.CrearYEjecutarRetiro(idMovimiento, cuentaOrigen, monto, descripcion)
            );
            Assert.Equal("monto", exception.ParamName);
        }

        [Fact]
        public void CrearYEjecutarRetiro_ConMontoMayorAlPermitido_DebeLanzarInvalidOperationException()
        {
            // Arrange - CA5: El monto máximo por movimiento no puede exceder 5000
            var cuentaOrigen = new CuentaAhorros("12345", 10000m, 0.01, new EstadoCuentaActiva());
            var montoExcesivo = 5001m; // El límite es 5000
            var idMovimiento = Guid.NewGuid().ToString();
            var descripcion = "Retiro de prueba";

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                _movimientoService.CrearYEjecutarRetiro(idMovimiento, cuentaOrigen, montoExcesivo, descripcion)
            );
            Assert.Contains("El monto excede el máximo permitido por movimiento", exception.Message);
        }

        [Fact]
        public void CrearYEjecutarRetiro_CuentaAhorros_ConSaldoInsuficiente_DebeLanzarInvalidOperationException()
        {
            // Arrange - CA3: Para cuentas de ahorros no se permite dejar saldo negativo
            var saldoInicial = 500m;
            var montoRetiro = 600m; // Más que el saldo
            var cuentaOrigen = new CuentaAhorros("12345", saldoInicial, 0.01, new EstadoCuentaActiva());
            var idMovimiento = Guid.NewGuid().ToString();
            var descripcion = "Retiro de prueba";

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                _movimientoService.CrearYEjecutarRetiro(idMovimiento, cuentaOrigen, montoRetiro, descripcion)
            );
            Assert.Equal("Fondos insuficientes.", exception.Message);
        }

        [Fact]
        public void CrearYEjecutarRetiro_CuentaCorriente_DentroDeLimiteSobregiro_DebePermitirRetiro()
        {
            // Arrange - CA2: Para cuentas corrientes, se permite saldo negativo hasta el límite de sobregiro
            var saldoInicial = 500m;
            var limiteSobregiro = 1000m;
            var montoRetiro = 800m; // Mayor que el saldo, pero dentro del límite de sobregiro
            var cuentaOrigen = new CuentaCorriente("67890", saldoInicial, limiteSobregiro, new EstadoCuentaActiva());
            var idMovimiento = Guid.NewGuid().ToString();
            var descripcion = "Retiro con sobregiro";

            // Act
            var movimiento = _movimientoService.CrearYEjecutarRetiro(idMovimiento, cuentaOrigen, montoRetiro, descripcion);

            // Assert
            Assert.NotNull(movimiento);
            Assert.Equal(saldoInicial - montoRetiro, cuentaOrigen.Saldo); // Saldo negativo
            Assert.True(cuentaOrigen.Saldo < 0); // Confirmar que el saldo es negativo
            Assert.True(cuentaOrigen.Saldo >= -limiteSobregiro); // Pero dentro del límite
        }

        [Fact]
        public void CrearYEjecutarRetiro_CuentaCorriente_ExcedeLimiteSobregiro_DebeLanzarInvalidOperationException()
        {
            // Arrange - CA2: El sobregiro tiene un límite
            var saldoInicial = 500m;
            var limiteSobregiro = 1000m;
            var montoRetiro = 2000m; // Excede saldo + sobregiro (500 + 1000 = 1500)
            var cuentaOrigen = new CuentaCorriente("67890", saldoInicial, limiteSobregiro, new EstadoCuentaActiva());
            var idMovimiento = Guid.NewGuid().ToString();
            var descripcion = "Retiro excesivo";

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                _movimientoService.CrearYEjecutarRetiro(idMovimiento, cuentaOrigen, montoRetiro, descripcion)
            );
            Assert.Equal("Excede límite de sobregiro.", exception.Message);
        }

        [Fact]
        public void CrearYEjecutarRetiro_CuentaAhorros_RetiroExactoDelSaldo_DebeDejaSaldoEnCero()
        {
            // Arrange - Caso borde: retirar exactamente todo el saldo
            var saldoInicial = 1000m;
            var montoRetiro = 1000m;
            var cuentaOrigen = new CuentaAhorros("12345", saldoInicial, 0.01, new EstadoCuentaActiva());
            var idMovimiento = Guid.NewGuid().ToString();
            var descripcion = "Retiro total";

            // Act
            var movimiento = _movimientoService.CrearYEjecutarRetiro(idMovimiento, cuentaOrigen, montoRetiro, descripcion);

            // Assert
            Assert.NotNull(movimiento);
            Assert.Equal(0m, cuentaOrigen.Saldo);
        }

        [Fact]
        public void CrearYEjecutarRetiro_VerificarTipoDeMovimiento_DebeSerRetiro()
        {
            // Arrange
            var cuentaOrigen = new CuentaAhorros("12345", 1000m, 0.01, new EstadoCuentaActiva());
            var montoRetiro = 200m;
            var idMovimiento = Guid.NewGuid().ToString();
            var descripcion = "Retiro de prueba";

            // Act
            var movimiento = _movimientoService.CrearYEjecutarRetiro(idMovimiento, cuentaOrigen, montoRetiro, descripcion);

            // Assert
            Assert.Equal("RETIRO", movimiento.Tipo);
            Assert.Equal(cuentaOrigen, movimiento.Origen);
            Assert.Equal(cuentaOrigen, movimiento.Destino); // En retiro, origen y destino son la misma cuenta
        }

        #endregion
    }
}
