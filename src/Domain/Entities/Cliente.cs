using Domain.Interfaces.States;
using System;

namespace Domain.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class Cliente
    {
        [Key]
        public string Cedula { get; private set; }
        public string Nombre { get; private set; }
        public string Apellido { get; private set; }
        public string Direccion { get; private set; }
        public string Correo { get; private set; }
        public string Telefono { get; private set; }

        // Navigation: un cliente puede tener una sola cuenta
        public Cuenta? Cuenta { get; private set; }

        // Parameterless constructor for EF Core
        protected Cliente()
        {
            Cedula = string.Empty;
            Nombre = string.Empty;
            Apellido = string.Empty;
            Direccion = string.Empty;
            Correo = string.Empty;
            Telefono = string.Empty;
            Cuenta = null;
        }

        public Cliente(string cedula, string nombre, string apellido, string direccion, string correo, string telefono)
        {
            Cedula = cedula;
            Nombre = nombre;
            Apellido = apellido;
            Direccion = direccion;
            Correo = correo;
            Telefono = telefono;
            Cuenta = null;
        }

        public static Cliente Create(string cedula, string nombre, string apellido, string direccion, string correo, string telefono)
        {
            if (string.IsNullOrWhiteSpace(cedula)) throw new ArgumentException("Cedula inválida.", nameof(cedula));
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Nombre inválido.", nameof(nombre));
            if (string.IsNullOrWhiteSpace(apellido)) throw new ArgumentException("Apellido inválido.", nameof(apellido));
            if (string.IsNullOrWhiteSpace(direccion)) throw new ArgumentException("Dirección inválida.", nameof(direccion));
            if (string.IsNullOrWhiteSpace(correo)) throw new ArgumentException("Correo inválido.", nameof(correo));
            if (string.IsNullOrWhiteSpace(telefono)) throw new ArgumentException("Teléfono inválido.", nameof(telefono));

            return new Cliente(cedula, nombre, apellido, direccion, correo, telefono);
        }

        // Crear y asociar una cuenta de ahorros al cliente (solo si no tiene una cuenta)
        public CuentaAhorros CrearCuentaAhorros(string numeroCuenta, decimal saldoInicial, double tasaInteres, Interfaces.States.IEstadoCuenta estadoInicial)
        {
            if (Cuenta != null) throw new InvalidOperationException("El cliente ya tiene una cuenta.");

            var cuenta = CuentaAhorros.Create(numeroCuenta, saldoInicial, tasaInteres, estadoInicial);
            cuenta.SetCliente(this);
            Cuenta = cuenta;
            return cuenta;
        }

        // Crear y asociar una cuenta corriente al cliente (solo si no tiene una cuenta)
        public CuentaCorriente CrearCuentaCorriente(string numeroCuenta, decimal saldoInicial, decimal limiteSobregiro, Interfaces.States.IEstadoCuenta estadoInicial)
        {
            if (Cuenta != null) throw new InvalidOperationException("El cliente ya tiene una cuenta.");

            var cuenta = CuentaCorriente.Create(numeroCuenta, saldoInicial, limiteSobregiro, estadoInicial);
            cuenta.SetCliente(this);
            Cuenta = cuenta;
            return cuenta;
        }

        // Actualiza los datos del cliente validando los parámetros.
        public void Update(string nombre, string apellido, string direccion, string correo, string telefono)
        {
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Nombre inválido.", nameof(nombre));
            if (string.IsNullOrWhiteSpace(apellido)) throw new ArgumentException("Apellido inválido.", nameof(apellido));
            if (string.IsNullOrWhiteSpace(direccion)) throw new ArgumentException("Dirección inválida.", nameof(direccion));
            if (string.IsNullOrWhiteSpace(correo)) throw new ArgumentException("Correo inválido.", nameof(correo));
            if (string.IsNullOrWhiteSpace(telefono)) throw new ArgumentException("Teléfono inválido.", nameof(telefono));

            Nombre = nombre;
            Apellido = apellido;
            Direccion = direccion;
            Correo = correo;
            Telefono = telefono;
        }
    }
}
