using Domain.Interfaces.Types;
using Domain.Logic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Movimiento
    {
        [Key]
        public string IdMovimiento { get; private set; }
        public decimal Monto { get; private set; }
        public DateTime Fecha { get; private set; }
        public string Descripcion { get; private set; }

        public string Tipo { get; private set; }

        [NotMapped]
        public ITipoMovimiento Estrategia { get; private set; }

        // Relaciones de EF Core
        public Cuenta? Origen { get; private set; }
        public Cuenta Destino { get; private set; }

        // --- CONSTRUCTORES ---

        public static Movimiento Create(string idMovimiento, decimal monto, Cuenta? origen, Cuenta destino, string descripcion, ITipoMovimiento estrategia)
        {
            if (string.IsNullOrWhiteSpace(idMovimiento)) throw new ArgumentException("IdMovimiento inválido.");
            if (monto <= 0) throw new ArgumentOutOfRangeException(nameof(monto), "El monto debe ser > 0.");
            if (estrategia == null) throw new ArgumentNullException(nameof(estrategia));
            if (destino == null) throw new ArgumentNullException(nameof(destino));

            return new Movimiento
            {
                IdMovimiento = idMovimiento,
                Monto = monto,
                Origen = origen,
                Destino = destino,
                Descripcion = descripcion,
                Fecha = DateTime.UtcNow,
                Estrategia = estrategia,
                Tipo = ObtenerNombreDeEstrategia(estrategia)
            };
        }

        // Constructor PROTEGIDO para EF Core (Materialización)
        protected Movimiento(string idMovimiento, decimal monto, string tipo, string descripcion, DateTime fecha)
        {
            IdMovimiento = idMovimiento;
            Monto = monto;
            Descripcion = descripcion;
            Fecha = fecha;
            Tipo = tipo;
            Destino = null!;
            Estrategia = ResolverEstrategia(tipo);
        }

        // Necesario para EF Core en algunos casos de proxy
        protected Movimiento()
        {
            IdMovimiento = string.Empty;
            Descripcion = string.Empty;
            Tipo = string.Empty;
            Estrategia = null!;
            Destino = null!;
        }

        // --- MÉTODOS AUXILIARES ---

        private static ITipoMovimiento ResolverEstrategia(string tipo)
        {
            return tipo?.ToUpper() switch
            {
                "DEPOSITO" => new DepositoTipo(),
                "RETIRO" => new RetiroTipo(),
                "TRANSFERENCIA" => new TransferenciaTipo(),
                "INTERES" => new InteresTipo(),
                _ => throw new InvalidOperationException($"Tipo de movimiento desconocido en la BD: {tipo}")
            };
        }

        private static string ObtenerNombreDeEstrategia(ITipoMovimiento estrategia)
        {
            return estrategia switch
            {
                DepositoTipo => "DEPOSITO",
                RetiroTipo => "RETIRO",
                TransferenciaTipo => "TRANSFERENCIA",
                InteresTipo => "INTERES",
                _ => throw new ArgumentException("Estrategia no reconocida")
            };
        }

        public void Ejecutar()
        {
            if (Estrategia == null)
                throw new InvalidOperationException("La estrategia no se ha cargado.");

            Estrategia.procesar(this);
        }
    }
}
