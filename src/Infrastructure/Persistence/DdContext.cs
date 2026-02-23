using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Fast_Bank.Infrastructure.Persistence
{
    public class DdContext : DbContext, IDdContext
    {
        public DdContext(DbContextOptions<DdContext> options)
            : base(options)
        {
            // Al cargar entidades desde la BD, reconstruir el objeto State
            ChangeTracker.Tracked += (sender, e) =>
            {
                if (e.Entry.Entity is Cuenta cuenta)
                {
                    cuenta.ReconstruirEstadoDesdeBD();
                }
            };
        }

        // DbSet por cada entidad del dominio que necesites persistir
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Movimiento> Movimientos { get; set; }
        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<CuentaAhorros> CuentasAhorros { get; set; }
        public DbSet<CuentaCorriente> CuentasCorrientes { get; set; }
        public DbSet<TarjetaCredito> TarjetasCredito { get; set; }
        // public DbSet<EntidadFinanciera> EntidadesFinancieras { get; set; } // Comentado: clase no existe en el proyecto
        public DbSet<ControlEjecucion> ControlEjecuciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuraciones específicas de EF Core (TPH para la jerarquía de Cuenta)
            modelBuilder.Entity<Cuenta>(b =>
            {
                b.ToTable("Cuentas");
                b.HasDiscriminator<string>("TipoCuenta")
                    .HasValue<CuentaAhorros>("Ahorros")
                    .HasValue<CuentaCorriente>("Corriente");

                // Ignorar el campo privado _estado (es un objeto State Pattern en memoria, no se persiste)
                b.Ignore("_estado");

                // Persistir el estado como columna string
                b.Property(c => c.Estado)
                    .IsRequired()
                    .HasDefaultValue("Activa");

                // Configurar la navegación inversa desde el Agregado hacia sus Movimientos.
                // EF Core usará el backing field _movimientos para cargar/rastrear la colección.
                b.HasMany(c => c.Movimientos)
                    .WithOne(m => m.Destino)
                    .IsRequired(false);  // Nullable: retiros no tienen cuenta destino
                b.Navigation(c => c.Movimientos)
                    .UsePropertyAccessMode(PropertyAccessMode.Field);
            });

            // Configurar relación uno-a-uno entre Cliente y Cuenta especificando la FK
            modelBuilder.Entity<Movimiento>(b =>
            {
                // Origen es nullable: depósitos no tienen cuenta origen
                b.HasOne(m => m.Origen)
                    .WithMany()
                    .HasForeignKey("OrigenNumeroCuenta")
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configurar relación uno-a-uno entre Cliente y Cuenta especificando la FK
            modelBuilder.Entity<Cliente>()
                .HasOne(c => c.Cuenta)
                .WithOne(cu => cu.Cliente)
                .HasForeignKey<Cuenta>(cu => cu.ClienteCedula);

            // Configurar relación uno-a-uno entre Cliente y TarjetaCredito
            modelBuilder.Entity<Cliente>()
                .HasOne(c => c.TarjetaCredito)
                .WithOne(t => t.Cliente)
                .HasForeignKey<TarjetaCredito>(t => t.IdCliente)
                .OnDelete(DeleteBehavior.Cascade);

            // Registrar las entidades concretas para asegurarnos que EF Core las descubre
            modelBuilder.Entity<CuentaAhorros>();
            modelBuilder.Entity<CuentaCorriente>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
