using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Masticore.Entity.Tests
{
    [TestClass()]
    public class EntityMergeCrudTests
    {
        [TestMethod()]
        public async Task CreateAsyncTest()
        {
            // Arrange
            var crud = await CreateCrud();
            var model = new TestModel { MergedString = "Erik Ralston", NotMergedString = "This Should Be Null" };

            // Act
            var createdModel = await crud.CreateAsync(model);

            // Assert
            Assert.AreEqual(createdModel.MergedString, model.MergedString);
            Assert.IsNull(createdModel.NotMergedString);
        }

        [TestMethod()]
        public async Task ReadAllAsyncTest()
        {
            // Arrange
            var crud = await CreateCrud();
            await crud.CreateAsync(new TestModel { MergedString = "Erik Ralston", NotMergedString = "This Should Be Null" });
            await crud.CreateAsync(new TestModel { MergedString = "Evee Ralston", NotMergedString = "This Should Be Null" });
            await crud.CreateAsync(new TestModel { MergedString = "Lilly Ralston", NotMergedString = "This Should Be Null" });

            // Act
            var models = await crud.ReadAllAsync();

            // Assert
            Assert.IsNotNull(models);
            Assert.AreEqual(models.Count(), 3);
        }

        [TestMethod()]
        public async Task ReadAsyncTest()
        {
            // Arrange
            var crud = await CreateCrud();
            var model = new TestModel { MergedString = "Erik Ralston", NotMergedString = "This Should Be Null" };
            var createdModel = await crud.CreateAsync(model);

            // Act
            var readModel = await crud.ReadAsync(createdModel.Id);

            // Assert
            Assert.IsNotNull(createdModel);
            Assert.IsNotNull(readModel);
        }

        [TestMethod()]
        public async Task UpdateAsyncTest()
        {
            // Arrange
            var crud = await CreateCrud();
            var originalModel = new TestModel { MergedString = "Erik Ralston", NotMergedString = "This Should Be Null" };
            var createdModel = await crud.CreateAsync(originalModel);
            createdModel.MergedString = "Evee Ralston";

            // Act
            var updatedModel = await crud.UpdateAsync(createdModel);

            // Assert
            Assert.IsNotNull(createdModel);
            Assert.IsNotNull(updatedModel);
            Assert.AreEqual(updatedModel.MergedString, createdModel.MergedString);
            Assert.AreNotEqual(updatedModel.MergedString, originalModel.MergedString);
        }

        [TestMethod()]
        public async Task DeleteAsyncTest()
        {
            // Arrange
            var crud = await CreateCrud();
            var originalModel = new TestModel { MergedString = "Erik Ralston", NotMergedString = "This Should Be Null" };
            var createdModel = await crud.CreateAsync(originalModel);

            // Act
            await crud.DeleteAsync(createdModel.Id);

            // Assert
            var readModel = await crud.ReadAsync(createdModel.Id);
            Assert.IsNull(readModel);
        }

        private static async Task<EntityMergeCrud<TestModel, TestDbContext>> CreateCrud()
        {
            var provider = new TestDbContextProvider();
            var context = provider.GetContext();
            context.Models.RemoveRange(context.Models);
            var crud = new TestCrud(provider);
            await context.SaveChangesAsync();
            return crud;
        }

        private class TestCrud : EntityMergeCrud<TestModel, TestDbContext>
        {
            public TestCrud(TestDbContextProvider provider) : base(provider, false) {  }
        }

        private class TestDbContextProvider : DbContextProvider<TestDbContext>
        {
            protected override TestDbContext CreateContext()
            {
                return new TestDbContext();
            }
        }
    }
}