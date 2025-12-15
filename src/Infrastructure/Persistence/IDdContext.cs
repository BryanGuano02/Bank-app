using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Fast_Bank.Infrastructure.Persistence
{
    public interface IDdContext
    {
        DbSet<Cliente> Clientes { get; }
        DbSet<Movimiento> Movimientos { get; }
        DbSet<Cuenta> Cuentas { get; }
        DbSet<CuentaAhorros> CuentasAhorros { get; }
        DbSet<CuentaCorriente> CuentasCorrientes { get; }
        DbSet<TarjetaCredito> TarjetasCredito { get; }
        DbSet<EntidadFinanciera> EntidadesFinancieras { get; }
        DbSet<ControlEjecucion> ControlEjecuciones { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
