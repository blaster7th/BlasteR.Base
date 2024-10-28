using System;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace BlasteR.Base.Tests
{
    public class TestFixture : IDisposable
    {
        public TestContext DB { get; set; }

        private static object lockObject = new object();

        public TestFixture()
        {
            lock (lockObject)
            {
                DB = DbContextFactory.New();
            }
        }

        public void Dispose()
        {
            DB.Dispose();
        }
    }
}
