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
    public class EntityMergeCrudTests
    {
        [TestMethod()]
        public async Task CreateAsyncTest()
        {
            // Arrange
            EntityMergeCrud<TestModel, TestDbContext> crud = await CreateCrud();
            TestModel model = new TestModel { MergedString = "Erik Ralston", NotMergedString = "This Should Be Null" };

            // Act
            TestModel createdModel = await crud.CreateAsync(model);

            // Assert
            Assert.AreEqual(createdModel.MergedString, model.MergedString);
            Assert.IsNull(createdModel.NotMergedString);
        }

        [TestMethod()]
        public async Task ReadAllAsyncTest()
        {
            // Arrange
            EntityMergeCrud<TestModel, TestDbContext> crud = await CreateCrud();
            await crud.CreateAsync(new TestModel { MergedString = "Erik Ralston", NotMergedString = "This Should Be Null" });
            await crud.CreateAsync(new TestModel { MergedString = "Evee Ralston", NotMergedString = "This Should Be Null" });
            await crud.CreateAsync(new TestModel { MergedString = "Lilly Ralston", NotMergedString = "This Should Be Null" });

            // Act
            IEnumerable<TestModel> models = await crud.ReadAllAsync();

            // Assert
            Assert.IsNotNull(models);
            Assert.AreEqual(models.Count(), 3);
        }

        [TestMethod()]
        public async Task ReadAsyncTest()
        {
            // Arrange
            EntityMergeCrud<TestModel, TestDbContext> crud = await CreateCrud();
            TestModel model = new TestModel { MergedString = "Erik Ralston", NotMergedString = "This Should Be Null" };
            TestModel createdModel = await crud.CreateAsync(model);

            // Act
            TestModel readModel = await crud.ReadAsync(createdModel.Id);

            // Assert
            Assert.IsNotNull(createdModel);
            Assert.IsNotNull(readModel);
        }

        [TestMethod()]
        public async Task UpdateAsyncTest()
        {
            // Arrange
            EntityMergeCrud<TestModel, TestDbContext> crud = await CreateCrud();
            TestModel originalModel = new TestModel { MergedString = "Erik Ralston", NotMergedString = "This Should Be Null" };
            TestModel createdModel = await crud.CreateAsync(originalModel);
            createdModel.MergedString = "Evee Ralston";

            // Act
            TestModel updatedModel = await crud.UpdateAsync(createdModel);

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
            EntityMergeCrud<TestModel, TestDbContext> crud = await CreateCrud();
            TestModel originalModel = new TestModel { MergedString = "Erik Ralston", NotMergedString = "This Should Be Null" };
            TestModel createdModel = await crud.CreateAsync(originalModel);

            // Act
            await crud.DeleteAsync(createdModel.Id);

            // Assert
            TestModel readModel = await crud.ReadAsync(createdModel.Id);
            Assert.IsNull(readModel);
        }

        #region Supporting Methods

        private static async Task<EntityMergeCrud<TestModel, TestDbContext>> CreateCrud()
        {
            EntityMergeCrud<TestModel, TestDbContext> crud = new EntityMergeCrud<TestModel, TestDbContext>();
            var context = new TestDbContext();
            context.Models.RemoveRange(context.Models);
            await context.SaveChangesAsync();
            crud.SetContext(context);
            return crud;
        }

        #endregion
    }
}