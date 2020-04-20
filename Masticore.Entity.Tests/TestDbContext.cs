using System.Data.Entity;

namespace Masticore.Entity.Tests
{
    public class TestModel : IIdentifiable<int>
    {
        public int Id { get; set; }

        public string NotMergedString { get; set; }

        [Merge]
        public string MergedString { get; set; }
    }

    public class TestSingletonModel : IIdentifiable<int>
    {
        public int Id { get; set; }

        public string NotMergedString { get; set; }

        [Merge]
        public string MergedString { get; set; }
    }

    public class TestDbContext : DbContext
    {
        public DbSet<TestModel> Models { get; set; }

        public DbSet<TestSingletonModel> SingletonModel { get; set; }
    }
}
