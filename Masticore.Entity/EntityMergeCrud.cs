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
    /// <typeparam name="ModelType"></typeparam>
    /// <typeparam name="DbContextType"></typeparam>
    public class EntityMergeCrud<ModelType, DbContextType> : MergeCrudBase<ModelType, int>
        where ModelType : class, IIdentifiable<int>, new()
        where DbContextType : DbContext
    {
        /// <summary>
        /// Gets the DbContext 
        /// </summary>
        public virtual DbContextType DbContext { get; set; }

        /// <summary>
        /// Gets or sets the IQueryable for read operations in this object
        /// </summary>
        public virtual IQueryable<ModelType> Queryable { get; set; }

        /// <summary>
        /// Gets or sets the flag indicating if Create, Update, and Delete automatically calls SaveChanges on the DbContext
        /// </summary>
        public virtual bool IsAutoSave { get; set; } = true;

        /// <summary>
        /// Gets the set in this context for the ModelType
        /// </summary>
        public virtual DbSet<ModelType> DbSet
        {
            get
            {
                return DbContext.Set<ModelType>();
            }
        }

        /// <summary>
        /// Gets the context for this object, based on the given DbContext and optionally the given queryable for read values
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="queryable">queryable for read operations; if null, this defaults to the set for the ModelType</param>
        public void SetContext(DbContextType dbContext, IQueryable<ModelType> queryable = null)
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));

            DbContext = dbContext;

            if (queryable == null)
                Queryable = DbSet;
            else
                Queryable = queryable;
        }

        /// <summary>
        /// Saves changes to the DbContext, but only is IsAutoSave == true
        /// </summary>
        /// <returns></returns>
        protected async Task AutoSave()
        {
            if (IsAutoSave)
                await DbContext.SaveChangesAsync();
        }

        #region ICrudAsync Implementation

        /// <summary>
        /// Creates a new model based on the given template model and the MergeModel strategy
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override async Task<ModelType> CreateAsync(ModelType model)
        {
            ModelType newModel = await base.CreateAsync(model);
            DbSet.Add(newModel);
            await AutoSave();

            return newModel;
        }

        /// <summary>
        /// Gets all items in the Queryable for this ModelType
        /// </summary>
        /// <returns></returns>
        public override async Task<IEnumerable<ModelType>> ReadAllAsync()
        {
            return (await Queryable.ToArrayAsync()) as IEnumerable<ModelType>;
        }

        /// <summary>
        /// Gets the specific entity model for the given ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Task<ModelType> ReadAsync(int id)
        {
            return Queryable.Where(m => m.Id == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Updates the model corresponding to the given model in the repository
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override async Task<ModelType> UpdateAsync(ModelType model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            ModelType existingModel = await base.UpdateAsync(model);
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
            ModelType model = await this.ReadAsync(id);
            DbSet.Remove(model);
            await AutoSave();
        }

        #endregion
    }
}
