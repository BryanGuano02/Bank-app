using System;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Patterns.State;
using Fast_Bank.Infrastructure.Persistence;

namespace Fast_Bank.Application.Services
{
    public class CuentaService
    {
        private readonly IDdContext _context;

        public CuentaService(IDdContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

    }
}
