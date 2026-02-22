using Domain.Entities;
using Domain.Interfaces.Repositories;
using Fast_Bank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fast_Bank.Infrastructure.Repositories
{
    public class CuentaAhorroRepository : ICuentaAhorroRepository
    {
        private readonly IDdContext _context;

        public CuentaAhorroRepository(IDdContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CuentaAhorros>> GetCuentasConSaldoPositivoAsync(int page, int pageSize)
        {
            return await _context.CuentasAhorros
                .Where(c => c.Saldo > 0)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
