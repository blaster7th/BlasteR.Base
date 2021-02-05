using Microsoft.EntityFrameworkCore;

namespace BlasteR.Base.Tests
{
    public static class DbContextFactory
    {
        public static TestContext New()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TestContext>();

            string connectionString = "Server=(localdb)\\mssqllocaldb;Database=TestDB;Trusted_Connection=True;MultipleActiveResultSets=true";

            optionsBuilder.UseSqlServer(connectionString);

            return new TestContext(optionsBuilder.Options);
        }
    }
}