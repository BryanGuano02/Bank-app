using System;
using Domain.Entities;
using Domain.Services;
using Domain.Patterns.State;
using Xunit;

namespace Fast.Bank.Application.Tests.Domain
{
    public class InteresesServiceTests
    {
        private readonly InteresesService _service;

        public InteresesServiceTests()
        {
            _service = new InteresesService();
        }

        [Fact]
        public void CalcularInteresMensual_ConDatosValidos_RetornaInteresRedondeado()
        {
            // Arrange
            var cuenta = CuentaAhorros.Create("A100", 1000m, new EstadoCuentaActiva());

            // Act
            var interes = _service.CalcularInteresMensual(cuenta);

            // Assert
            Assert.Equal(2.5m, interes); // 1000 * 3% / 12 = 2.5
        }

        [Fact]
        public void CrearYEjecutarAcreditacionInteres_ConDatosValidos_AcreditaSaldo()
        {
            // Arrange
            var cuenta = CuentaAhorros.Create("A200", 500m, new EstadoCuentaActiva());
            var id = Guid.NewGuid().ToString();
            var monto = 5.12m;

            // Act
            var movimiento = _service.CrearYEjecutarAcreditacionInteres(id, cuenta, monto);

            // Assert
            Assert.NotNull(movimiento);
            Assert.Equal(monto, movimiento.Monto);
            Assert.Equal(500m + monto, cuenta.Saldo);
            Assert.Contains("Inter�s mensual", movimiento.Descripcion);
            Assert.Equal(cuenta, movimiento.Destino);
        }

        [Fact]
        public void CrearYEjecutarAcreditacionInteres_CuentaNula_DebeLanzarArgumentNullException()
        {
            var id = Guid.NewGuid().ToString();
            var monto = 1m;

            Assert.Throws<ArgumentNullException>(() => _service.CrearYEjecutarAcreditacionInteres(id, null!, monto));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CrearYEjecutarAcreditacionInteres_IdInvalido_DebeLanzarArgumentException(string id)
        {
            var cuenta = CuentaAhorros.Create("A300", 100m, new EstadoCuentaActiva());
            var monto = 1m;

            Assert.Throws<ArgumentException>(() => _service.CrearYEjecutarAcreditacionInteres(id!, cuenta, monto));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void CrearYEjecutarAcreditacionInteres_MontoInvalido_DebeLanzarArgumentOutOfRange(decimal monto)
        {
            var cuenta = CuentaAhorros.Create("A400", 100m, new EstadoCuentaActiva());
            var id = Guid.NewGuid().ToString();

            Assert.Throws<ArgumentOutOfRangeException>(() => _service.CrearYEjecutarAcreditacionInteres(id, cuenta, monto));
        }
    }
}
