using Microsoft.VisualStudio.TestTools.UnitTesting;
using Masticore.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masticore.Entity.Tests
{
    [TestClass()]
    public class EntityMergeSingletonCrudTests
    {
        [TestMethod()]
        public async Task CreateOrUpdateAsyncTest()
        {
            // Arrange
            TestDbContext context;
            EntityMergeSingletonCrud<TestSingletonModel, TestDbContext> crud;
            CreateContextAndCrud(out context, out crud);

            // Act
            TestSingletonModel model = new TestSingletonModel { MergedString = "Just One Of These", NotMergedString = "Not Persisted" };
            TestSingletonModel createdModel = await crud.CreateOrUpdateAsync(model);
            createdModel.MergedString = "Only One";
            TestSingletonModel updatedModel = await crud.CreateOrUpdateAsync(model);

            // Assert
            Assert.IsNotNull(createdModel);
            Assert.IsNull(createdModel.NotMergedString);
            Assert.IsNotNull(updatedModel);
            Assert.AreEqual(createdModel.MergedString, updatedModel.MergedString);
            Assert.AreEqual(context.SingletonModel.Count(), 1);
        }

        [TestMethod()]
        public async Task DeleteAsyncTest()
        {
            // Arrange
            TestDbContext context;
            EntityMergeSingletonCrud<TestSingletonModel, TestDbContext> crud;
            CreateContextAndCrud(out context, out crud);
            TestSingletonModel model = new TestSingletonModel { MergedString = "Just One Of These", NotMergedString = "Not Persisted" };
            TestSingletonModel createdModel = await crud.CreateOrUpdateAsync(model);

            // Act
            await crud.DeleteAsync();

            // Assert
            TestSingletonModel readModel = await crud.ReadAsync();
            Assert.IsNull(readModel);
            Assert.AreEqual(context.SingletonModel.Count(), 0);
        }

        [TestMethod()]
        public async Task ReadAsyncTest()
        {
            // Arrange
            TestDbContext context;
            EntityMergeSingletonCrud<TestSingletonModel, TestDbContext> crud;
            CreateContextAndCrud(out context, out crud);
            TestSingletonModel model = new TestSingletonModel { MergedString = "Just One Of These", NotMergedString = "Not Persisted" };
            TestSingletonModel createdModel = await crud.CreateOrUpdateAsync(model);

            // Act
            TestSingletonModel readModel = await crud.ReadAsync();

            // Assert
            Assert.IsNotNull(readModel);
            Assert.AreEqual(model.MergedString, readModel.MergedString);
            Assert.IsNull(readModel.NotMergedString);
            Assert.AreEqual(context.SingletonModel.Count(), 1);
        }

        #region Supporting Methods

        private static void CreateContextAndCrud(out TestDbContext context, out EntityMergeSingletonCrud<TestSingletonModel, TestDbContext> crud)
        {
            context = new TestDbContext();
            crud = new EntityMergeSingletonCrud<TestSingletonModel, TestDbContext>();
            context.SingletonModel.RemoveRange(context.SingletonModel);
            context.SaveChanges();
            crud.DbContext = context;
        }

        #endregion
    }
}