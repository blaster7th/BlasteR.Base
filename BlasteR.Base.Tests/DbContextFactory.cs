using Microsoft.EntityFrameworkCore;

namespace BlasteR.Base.Tests
{
    public static class DbContextFactory
    {
        public static TestContext New()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TestContext>();

            string connectionString = string.Empty;
            bool useMariaDB = false;
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
            {
                // useMariaDB = true;
                // connectionString ="server=localhost;userid=dbuser;pwd=DBPass123;port=3306;database=TestDB;sslmode=none";
                connectionString = "Data Source=localhost;Initial Catalog=TestDB;User Id=BlasteR;Password=blastup7th";
            }
            else
            {
                connectionString = "Server=(localdb)\\mssqllocaldb;Database=TestDB;Trusted_Connection=True;MultipleActiveResultSets=true";
            }

            if (useMariaDB)
                optionsBuilder.UseMySql(connectionString);
            else
                optionsBuilder.UseSqlServer(connectionString);

            return new TestContext(optionsBuilder.Options);
        }
    }
}