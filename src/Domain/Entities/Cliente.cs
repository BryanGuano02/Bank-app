using Domain.Interfaces.States;
using System;

namespace Domain.Entities
{
    public class Cliente
    {
        public string Cedula { get; private set; }
        public string Nombre { get; private set; }
        public string Apellido { get; private set; }
        public string Direccion { get; private set; }
        public string Correo { get; private set; }
        public string Telefono { get; private set; }

        public IEstadoCliente Estado { get; private set; }

        public Cliente(string cedula, string nombre, string apellido, IEstadoCliente estadoInicial)
        {
            Cedula = cedula;
            Nombre = nombre;
            Apellido = apellido;
            Estado = estadoInicial;
        }

        public static Cliente Create(string cedula, string nombre, string apellido, IEstadoCliente estadoInicial)
        {
            if (string.IsNullOrWhiteSpace(cedula)) throw new ArgumentException("Cedula inválida.", nameof(cedula));
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Nombre inválido.", nameof(nombre));
            if (string.IsNullOrWhiteSpace(apellido)) throw new ArgumentException("Apellido inválido.", nameof(apellido));
            if (estadoInicial == null) throw new ArgumentNullException(nameof(estadoInicial));

            return new Cliente(cedula, nombre, apellido, estadoInicial);
        }

        public void CambiarEstado(IEstadoCliente nuevoEstado) => Estado = nuevoEstado;
    }
}
