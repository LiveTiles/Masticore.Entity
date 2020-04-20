using System.Data.Entity;

namespace Masticore.Entity
{
    public abstract class DbContextProvider<TContext> : IDbContextProvider<TContext> where TContext : DbContext
    {
        private TContext _context;
        public TContext GetContext()
        {
            return _context ?? (_context = CreateContext());
        }

        protected abstract TContext CreateContext();
    }
}
