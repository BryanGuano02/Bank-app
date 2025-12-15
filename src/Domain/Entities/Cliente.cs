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

        public Cuenta? Cuenta { get; private set; }

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

        internal void SetCuenta(Cuenta cuenta)
        {
            Cuenta = cuenta ?? throw new ArgumentNullException(nameof(cuenta));
        }
    }
}
