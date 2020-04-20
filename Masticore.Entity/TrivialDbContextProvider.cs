using System.Data.Entity;

namespace Masticore.Entity
{
    public class TrivialDbContextProvider<TContext> : IDbContextProvider<TContext>
        where TContext : DbContext
    {
        private readonly TContext _context;

        public TrivialDbContextProvider(TContext context)
        {
            _context = context;
        }
        public TContext GetContext()
        {
            return _context;
        }
    }
}