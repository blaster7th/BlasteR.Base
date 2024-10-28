using Microsoft.EntityFrameworkCore;

namespace BlasteR.Base.Tests
{
    public static class DbContextFactory
    {
        public static TestContext New()
        {
            var connection = new Microsoft.Data.Sqlite.SqliteConnection("DataSource=file::memory:?cache=shared");
            // var connection = new Microsoft.Data.Sqlite.SqliteConnection("DataSource=:memory:");
            connection.Open();
            var optionsBuilder = new DbContextOptionsBuilder<TestContext>();
            optionsBuilder.UseSqlite(connection);

            return new TestContext(optionsBuilder.Options);
        }
    }
}