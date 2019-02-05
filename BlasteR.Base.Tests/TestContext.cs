using Microsoft.EntityFrameworkCore;

namespace BlasteR.Base.Tests
{
    public class TestContext : DbContext
    {
        public TestContext(DbContextOptions<TestContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<TestEntity> TestEntities { get; set; }
        public DbSet<SecondEntity> SecondEntities { get; set; }
    }
}