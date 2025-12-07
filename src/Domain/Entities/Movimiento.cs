using Domain.Interfaces.States;
using System;

namespace Domain.Entities
{
    public class Movimiento
    {
        public string IdMovimiento { get; private set; }
        public decimal Monto { get; private set; }
        public DateTime Fecha { get; private set; }
        public string Descripcion { get; private set; }

        public Cuenta Origen { get; private set; }
        public Cuenta Destino { get; private set; }

        public Movimiento(string idMovimiento, decimal monto, Cuenta origen, Cuenta destino, string descripcion)
        {
            IdMovimiento = idMovimiento;
            Monto = monto;
            Origen = origen;
            Destino = destino;
            Descripcion = descripcion;
            Fecha = DateTime.UtcNow;
        }

        public static Movimiento Create(string idMovimiento, decimal monto, Cuenta origen, Cuenta destino, string descripcion)
        {
            if (string.IsNullOrWhiteSpace(idMovimiento)) throw new ArgumentException("IdMovimiento inválido.", nameof(idMovimiento));
            if (monto <= 0) throw new ArgumentOutOfRangeException(nameof(monto), "El monto debe ser mayor que cero.");
            if (origen == null) throw new ArgumentNullException(nameof(origen));
            if (destino == null) throw new ArgumentNullException(nameof(destino));

            return new Movimiento(idMovimiento, monto, origen, destino, descripcion);
        }
    }
}
