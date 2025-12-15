
using System;
using Domain.Entities;
using Domain.Logic;

namespace Domain.Services
{
    public class ClienteService
    {

        public ClienteService()
        {

        }

        public Cliente CrearCliente(string cedula, string nombre, string apellido, string direccion, string correo, string telefono)
        {
            return Cliente.Create(cedula, nombre, apellido, direccion, correo, telefono);
        }

        public Cliente ActualizarCliente(Cliente cliente, string nombre, string apellido, string direccion, string correo, string telefono)
        {
            if (cliente == null) throw new ArgumentNullException(nameof(cliente));

            cliente.Update(nombre, apellido, direccion, correo, telefono);

            return cliente;
        }
    }
}
