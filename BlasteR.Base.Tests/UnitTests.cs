using System.Linq;
using Xunit;

namespace BlasteR.Base.Tests
{
    public class UnitTests
    {
        [Fact]
        public void UpdateUsingEF()
        {
            using (var db = DbContextFactory.New())
            {
                TestEntity testEntity = new TestEntity()
                {
                    IntProperty = 1,
                    StringProperty = "Test"
                };

                db.TestEntities.Update(testEntity);
                db.SaveChanges();
            }

            TestEntity testFromDb = null;
            using (var db = DbContextFactory.New())
            {
                testFromDb = db.TestEntities.Last();
            }

            TestEntity deserialized = new TestEntity();
            deserialized.Id = testFromDb.Id;
            deserialized.CreatedTime = testFromDb.CreatedTime;
            deserialized.ModifiedTime = testFromDb.ModifiedTime;
            deserialized.IntProperty = testFromDb.IntProperty;
            deserialized.StringProperty = "Changed";

            using (var db = DbContextFactory.New())
            {
                db.TestEntities.Update(deserialized);

                db.SaveChanges();
            }
        }

        [Fact]
        public void InsertGraph()
        {
            using (var db = DbContextFactory.New())
            {
                TestBLL testBLL = new TestBLL(db);
                TestEntity testEntity = new TestEntity()
                {
                    IntProperty = 1,
                    StringProperty = "Test",
                    SecondEntity = new SecondEntity()
                    {
                        IntValue = 2,
                        StringValue = "Second"
                    }
                };

                testBLL.Save(testEntity);

                db.SaveChanges();
            }
        }
    }
}
