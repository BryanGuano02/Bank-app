using System;

namespace Domain.Entities
{
    public class EntidadFinanciera
    {
        public string IdEntidad { get; private set; }
        public string Nombre { get; private set; }
        public string Ciudad { get; private set; }
        public string Direccion { get; private set; }

        public EntidadFinanciera(string idEntidad, string nombre, string ciudad, string direccion)
        {
            IdEntidad = idEntidad;
            Nombre = nombre;
            Ciudad = ciudad;
            Direccion = direccion;
        }

        public static EntidadFinanciera Create(string idEntidad, string nombre, string ciudad, string direccion)
        {
            if (string.IsNullOrWhiteSpace(idEntidad)) throw new ArgumentException("IdEntidad inválido.", nameof(idEntidad));
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Nombre inválido.", nameof(nombre));
            if (string.IsNullOrWhiteSpace(ciudad)) throw new ArgumentException("Ciudad inválida.", nameof(ciudad));
            if (string.IsNullOrWhiteSpace(direccion)) throw new ArgumentException("Dirección inválida.", nameof(direccion));

            return new EntidadFinanciera(idEntidad, nombre, ciudad, direccion);
        }
    }
}
