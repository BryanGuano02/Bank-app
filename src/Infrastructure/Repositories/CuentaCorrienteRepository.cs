using Domain.Entities;
using Domain.Interfaces.Repositories;
using Fast_Bank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fast_Bank.Infrastructure.Repositories
{
    public class CuentaCorrienteRepository : ICuentaCorrienteRepository
    {
        private readonly IDdContext _context;

        public CuentaCorrienteRepository(IDdContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CuentaCorriente>> GetCuentasEnSobregiroAsync(int page, int pageSize)
        {
            return await _context.CuentasCorrientes
                .Where(c => c.Saldo < 0)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
