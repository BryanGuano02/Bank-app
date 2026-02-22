using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface ICuentaAhorroRepository
    {
        Task<IEnumerable<CuentaAhorros>> GetCuentasConSaldoPositivoAsync();
    }
}
