using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface ICuentaCorrienteRepository
    {
        Task<IEnumerable<CuentaCorriente>> GetCuentasEnSobregiroAsync(int page, int pageSize);
    }
}
