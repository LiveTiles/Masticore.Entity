using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;

namespace Masticore.Entity.Tests
{
    [TestClass]
    public class Initalize
    {
        [AssemblyInitialize]
        public static void InitializeDbContext(TestContext context)
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", context.TestDeploymentDir);
            Database.SetInitializer(new DropCreateDatabaseAlways<TestDbContext>());
        }
    }
}