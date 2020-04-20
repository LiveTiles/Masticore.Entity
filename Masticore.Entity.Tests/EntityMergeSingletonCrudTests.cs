using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
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
            var model = new TestSingletonModel { MergedString = "Just One Of These", NotMergedString = "Not Persisted" };
            var createdModel = await crud.CreateOrUpdateAsync(model);
            createdModel.MergedString = "Only One";
            var updatedModel = await crud.CreateOrUpdateAsync(model);

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
            var model = new TestSingletonModel { MergedString = "Just One Of These", NotMergedString = "Not Persisted" };
            var createdModel = await crud.CreateOrUpdateAsync(model);

            // Act
            await crud.DeleteAsync();

            // Assert
            var readModel = await crud.ReadAsync();
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
            var model = new TestSingletonModel { MergedString = "Just One Of These", NotMergedString = "Not Persisted" };
            var createdModel = await crud.CreateOrUpdateAsync(model);

            // Act
            var readModel = await crud.ReadAsync();

            // Assert
            Assert.IsNotNull(readModel);
            Assert.AreEqual(model.MergedString, readModel.MergedString);
            Assert.IsNull(readModel.NotMergedString);
            Assert.AreEqual(context.SingletonModel.Count(), 1);
        }

        private static void CreateContextAndCrud(out TestDbContext context, out EntityMergeSingletonCrud<TestSingletonModel, TestDbContext> crud)
        {
            context = new TestDbContext();
            crud = new TestEntityMergeSingleton(new TrivialDbContextProvider<TestDbContext>(context));
            context.SingletonModel.RemoveRange(context.SingletonModel);
            context.SaveChanges();
        }

        class TestEntityMergeSingleton : EntityMergeSingletonCrud<TestSingletonModel, TestDbContext>
        {
            public TestEntityMergeSingleton(IDbContextProvider<TestDbContext> provider) : base(provider)
            {
            }
        }
    }
}