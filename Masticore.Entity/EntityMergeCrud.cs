using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Masticore.Entity
{
    /// <summary>
    /// ICrudAsync implemented over a DbContext and IQueryable for ModelType 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TDbContext"></typeparam>
    public abstract class EntityMergeCrud<TModel, TDbContext> : MergeCrudBase<TModel, int>
        where TModel : class, IIdentifiable<int>, new()
        where TDbContext : DbContext
    {
        private readonly bool _filterSoftDeletable;

        protected EntityMergeCrud(IDbContextProvider<TDbContext> provider, bool filterSoftDeletable = true)
        {
            _provider = provider;
            _filterSoftDeletable = filterSoftDeletable && typeof(ISoftDeletable).IsAssignableFrom(typeof(TModel));
        }

        private readonly IDbContextProvider<TDbContext> _provider;
        /// <summary>
        /// Gets the DbContext 
        /// </summary>
        protected TDbContext DbContext => _provider.GetContext();

        /// <summary>
        /// Gets or sets the IQueryable for read operations in this object
        /// </summary>
        protected virtual IQueryable<TModel> Queryable
        {
            get
            {
                if (_filterSoftDeletable)
                {
                    return ((IQueryable<ISoftDeletable>) DbSet.AsQueryable())
                        .Where(d => !d.DeletedUtc.HasValue)
                        .Cast<TModel>();
                }

                return DbSet;
            }
        }

        /// <summary>
        /// Gets or sets the flag indicating if Create, Update, and Delete automatically calls SaveChanges on the DbContext
        /// </summary>
        public virtual bool IsAutoSave { get; set; } = true;

        /// <summary>
        /// Gets the set in this context for the ModelType
        /// </summary>
        public virtual DbSet<TModel> DbSet => DbContext.Set<TModel>();

        /// <summary>
        /// Saves changes to the DbContext, but only is IsAutoSave == true
        /// </summary>
        /// <returns></returns>
        protected virtual async Task AutoSave()
        {
            if (IsAutoSave)
                await Save();
            else
                await Task.CompletedTask;

        }
        /// <summary>
        /// Call this to manually save database changes. 
        /// </summary>
        /// <returns></returns>
        protected virtual async Task Save()
        {
            await DbContext.SaveChangesAsync();
        }


        /// <summary>
        /// Creates a new model based on the given template model and the MergeModel strategy
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override async Task<TModel> CreateAsync(TModel model)
        {
            var newModel = await base.CreateAsync(model);
            DbSet.Add(newModel);
            await AutoSave();

            return newModel;
        }

        /// <summary>
        /// Creates a new model based on the given template model and the MergeModel strategy
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override async Task<IEnumerable<TModel>> CreateAsync(IEnumerable<TModel> models)
        {
            var newModels = await base.CreateAsync(models);
            DbSet.AddRange(newModels);
            await AutoSave();

            return newModels;
        }
        /// <summary>
        /// Gets all items in the Queryable for this ModelType
        /// </summary>
        /// <returns></returns>
        public override async Task<IEnumerable<TModel>> ReadAllAsync()
        {
            return (await Queryable.ToArrayAsync()) as IEnumerable<TModel>;
        }

        /// <summary>
        /// Gets the specific entity model for the given ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Task<TModel> ReadAsync(int id)
        {
            return Queryable.Where(m => m.Id == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Updates the model corresponding to the given model in the repository
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override async Task<TModel> UpdateAsync(TModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var existingModel = await base.UpdateAsync(model);
            await AutoSave();

            return existingModel;
        }

        /// <summary>
        /// Deletes the model corresponding to the given ID from the repository
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task DeleteAsync(int id)
        {
            var model = await this.ReadAsync(id);
            DbSet.Remove(model);
            await AutoSave();
        }
        /// <summary>
        /// Deletes a set of models from the repository using the Id of the model.
        /// </summary>
        /// <param name="ids">An IEnumerable of the model ids to remove from the model</param>
        /// <returns></returns>
        public override async Task DeleteAsync(IEnumerable<int> ids)
        {
            var models = (await Queryable.Where(m => ids.Contains(m.Id)).ToArrayAsync()) as IEnumerable<TModel>;
            DbSet.RemoveRange(models);
            await AutoSave();
        }
    }
}
