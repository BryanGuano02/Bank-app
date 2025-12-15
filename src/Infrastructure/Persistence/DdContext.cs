using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Fast_Bank.Infrastructure.Persistence
{
    public class DdContext : DbContext, IDdContext
    {
        public DdContext(DbContextOptions<DdContext> options)
            : base(options)
        {
        }

        // DbSet por cada entidad del dominio que necesites persistir
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Movimiento> Movimientos { get; set; }
        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<CuentaAhorros> CuentasAhorros { get; set; }
        public DbSet<CuentaCorriente> CuentasCorrientes { get; set; }
        public DbSet<TarjetaCredito> TarjetasCredito { get; set; }
        public DbSet<EntidadFinanciera> EntidadesFinancieras { get; set; }
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
            });

            // Configurar relación uno-a-uno entre Cliente y Cuenta especificando la FK
            modelBuilder.Entity<Cliente>()
                .HasOne(c => c.Cuenta)
                .WithOne(cu => cu.Cliente)
                .HasForeignKey<Cuenta>(cu => cu.ClienteCedula);

            // Registrar las entidades concretas para asegurarnos que EF Core las descubre
            modelBuilder.Entity<CuentaAhorros>();
            modelBuilder.Entity<CuentaCorriente>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
