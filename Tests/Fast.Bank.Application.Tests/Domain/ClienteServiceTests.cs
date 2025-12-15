using System;
using Domain.Entities;
using Domain.Services;
using Domain.Patterns.State;
using Xunit;

namespace Fast.Bank.Application.Tests.Domain
{
    public class ClienteServiceTests
    {
        private readonly ClienteService _service;

        public ClienteServiceTests()
        {
            _service = new ClienteService();
        }

        [Fact]
        public void CrearCliente_ConDatosValidos_RetornaClienteConPropiedades()
        {
            // Arrange
            var cedula = "111222333";
            var nombre = "Juan";
            var apellido = "Perez";
            var direccion = "Calle Falsa 123";
            var correo = "juan.perez@example.com";
            var telefono = "3001234567";

            // Act
            var cliente = _service.CrearCliente(cedula, nombre, apellido, direccion, correo, telefono);

            // Assert
            Assert.NotNull(cliente);
            Assert.Equal(cedula, cliente.Cedula);
            Assert.Equal(nombre, cliente.Nombre);
            Assert.Equal(apellido, cliente.Apellido);
            Assert.Equal(direccion, cliente.Direccion);
            Assert.Equal(correo, cliente.Correo);
            Assert.Equal(telefono, cliente.Telefono);
            Assert.Null(cliente.Cuenta);
        }

        [Fact]
        public void CrearCliente_CedulaInvalida_DebeLanzarArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _service.CrearCliente("", "N", "A", "D", "e@e.com", "123"));
        }

        [Fact]
        public void ActualizarCliente_ConDatosValidos_ActualizaPropiedades()
        {
            var cliente = _service.CrearCliente("222333444", "Ana", "Lopez", "Av Siempreviva", "ana@example.com", "3000000000");

            // Act
            var actualizado = _service.ActualizarCliente(cliente, "Ana Maria", "Lopez Perez", "Nueva Direc", "ana.m@example.com", "3111111111");

            // Assert
            Assert.Equal("Ana Maria", actualizado.Nombre);
            Assert.Equal("Lopez Perez", actualizado.Apellido);
            Assert.Equal("Nueva Direc", actualizado.Direccion);
            Assert.Equal("ana.m@example.com", actualizado.Correo);
            Assert.Equal("3111111111", actualizado.Telefono);
        }

        [Fact]
        public void CrearCuentaAhorros_AsociaClienteYCuenta()
        {
            var cliente = _service.CrearCliente("333444555", "Luis", "Gomez", "Dir", "luis@example.com", "3009998888");

            var cuenta = _service.CrearCuentaAhorros(cliente, "AH-001", 100m, 1.2, new EstadoCuentaActiva());

            Assert.NotNull(cuenta);
            Assert.Equal("AH-001", cuenta.NumeroCuenta);
            Assert.Equal(cliente, cuenta.Cliente);
            Assert.Equal(cuenta, cliente.Cuenta);
        }

        [Fact]
        public void CrearCuentaCorriente_LimiteNegativo_DebeLanzarArgumentOutOfRange()
        {
            var cliente = _service.CrearCliente("444555666", "P", "Q", "D", "p@q.com", "3001112222");

            Assert.Throws<ArgumentOutOfRangeException>(() => _service.CrearCuentaCorriente(cliente, "CC-001", 0m, -100m));
        }
    }
}
