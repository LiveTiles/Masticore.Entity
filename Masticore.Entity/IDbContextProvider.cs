using System.Data.Entity;

namespace Masticore.Entity
{
    public interface IDbContextProvider<out TContext>
    {
        TContext GetContext();
    }
}