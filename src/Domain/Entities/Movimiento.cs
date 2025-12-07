using Domain.Interfaces.Types; // Asegúrate de tener este using para ITipoMovimiento
using Domain.Logic; // RetiroTipo, TransferenciaTipo, InteresTipo, DepositoTipo
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

        // 2. Esta es tu Estrategia (El objeto real).
        // [NotMapped] indica a EF Core que ignore esta propiedad para la tabla SQL.
        [NotMapped]
        public ITipoMovimiento Estrategia { get; private set; }

        // Relaciones de EF Core
        public Cuenta? Origen { get; private set; }
        public Cuenta Destino { get; private set; }

        // --- CONSTRUCTORES ---

        // Constructor para CREAR movimientos nuevos desde el código (Application Service)
        // Aquí recibes la Estrategia ya instanciada y guardamos su nombre en 'Tipo'
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

                // Asignamos la estrategia y el string para la BD
                Estrategia = estrategia,
                Tipo = ObtenerNombreDeEstrategia(estrategia)
            };
        }

        // Constructor PROTEGIDO para EF Core (Materialización)
        // EF llamará a este constructor cuando lea de la BD. Le pasará el valor de la columna 'Tipo'.
        protected Movimiento(string idMovimiento, decimal monto, string tipo, string descripcion, DateTime fecha)
        {
            IdMovimiento = idMovimiento;
            Monto = monto;
            Descripcion = descripcion;
            Fecha = fecha;
            Tipo = tipo;

            // ¡AQUÍ OCURRE LA MAGIA AUTOMÁTICA!
            // Convertimos el string "RETIRO" -> new RetiroTipo()
            Estrategia = ResolverEstrategia(tipo);
        }

        // Necesario para EF Core en algunos casos de proxy, pero intentará usar el parametrizado arriba
        protected Movimiento() { }


        // --- MÉTODOS AUXILIARES (FACTORIES INTERNAS) ---

        // Convierte el Texto de la BD a la Clase Estrategia
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

        // Convierte la Clase Estrategia al Texto para la BD
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

        // Método para ejecutar la lógica (El Service llama a esto)
        public void Ejecutar()
        {
            if (Estrategia == null)
                throw new InvalidOperationException("La estrategia no se ha cargado.");

            Estrategia.procesar(this); // Pasamos 'this' (el movimiento completo)
        }
    }
}
