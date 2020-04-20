using System.Data.Entity;
using System.Threading.Tasks;

namespace Masticore.Entity
{
    /// <summary>
    /// ISingletonCrud over EntityFramework
    /// DbContext must be set to an instance for items to work
    /// </summary>
    /// <typeparam name="ModelType"></typeparam>
    /// <typeparam name="DbContextType"></typeparam>
    public abstract class EntityMergeSingletonCrud<ModelType, DbContextType> : ISingletonCrud<ModelType>
        where ModelType : class, IIdentifiable<int>, new()
        where DbContextType : DbContext
    {
        private readonly IDbContextProvider<DbContextType> _provider;

        protected EntityMergeSingletonCrud(IDbContextProvider<DbContextType> provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Gets or sets the DbContext instance for this object
        /// </summary>
        protected DbContextType DbContext => _provider.GetContext();

        /// <summary>
        /// Gets the DbSet underlying this singleton
        /// </summary>
        protected DbSet<ModelType> DbSet => DbContext.Set<ModelType>();

        /// <summary>
        /// Gets or sets the flag indicating if Create, Update, and Delete automatically calls SaveChanges on the DbContext
        /// </summary>
        public virtual bool IsAutoSave { get; set; } = true;

        /// <summary>
        /// Will async save the database if IsAutoSave is true
        /// </summary>
        /// <returns></returns>
        protected virtual async Task AutoSaveAsync()
        {
            if (IsAutoSave)
                await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Async create or update the singleton instance based on the given template model, returning the singleton model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ModelType> CreateOrUpdateAsync(ModelType model)
        {
            var existingModel = await ReadAsync();
            if (existingModel == null)
            {
                existingModel = MergeCrudBase<ModelType, int>.Create(model);
                DbSet.Add(existingModel);
            }
            else
            {
                MergeCrudBase<ModelType, int>.Update(existingModel, model);
            }

            await AutoSaveAsync();

            return existingModel;
        }

        /// <summary>
        /// Deletes the singleton model in the system
        /// If the singleton cannot be read, then this does nothing
        /// </summary>
        /// <returns></returns>
        public virtual async Task DeleteAsync()
        {
            var existingModel = await ReadAsync();
            if (existingModel == null)
                return;

            DbSet.Remove(existingModel);
            await AutoSaveAsync();
        }

        /// <summary>
        /// Returns the singleton model
        /// NOTE: If the model is not created yet, this will return null
        /// </summary>
        /// <returns></returns>
        public virtual Task<ModelType> ReadAsync()
        {
            return DbSet.FirstOrDefaultAsync();
        }
    }
}
