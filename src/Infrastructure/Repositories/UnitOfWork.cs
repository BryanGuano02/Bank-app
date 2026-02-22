using Domain.Interfaces;
using Fast_Bank.Infrastructure.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace Fast_Bank.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDdContext _context;

        public UnitOfWork(IDdContext context)
        {
            _context = context;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => _context.SaveChangesAsync(cancellationToken);
    }
}
