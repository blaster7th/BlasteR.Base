using System.Linq;
using Xunit;

namespace BlasteR.Base.Tests
{
    public class UnitTests : IClassFixture<TestFixture>
    {
        public TestContext DB { get; set; }
        public UnitTests(TestFixture fixture)
        {
            DB = fixture.DB;
        }

        [Fact]
        public void CRUD()
        {
            // Arrange
            TestBll testBll = new TestBll(DB);
            var entity = new TestEntity()
            {
                IntProperty = 1,
                StringProperty = "Test"
            };

            // Act CREATE
            testBll.Save(entity, true);

            // Assert
            int entitiesCount = DB.TestEntities.Count();
            Assert.NotEqual(0, entity.Id);
            Assert.NotEqual(0, entitiesCount);

            // Act READ
            entity = testBll.GetById(entity.Id);

            // Assert
            Assert.NotNull(entity);

            // Act UPDATE
            entity.StringProperty = "Test Updated";
            testBll.Save(entity, true);

            // Assert
            entity = testBll.GetById(entity.Id);
            Assert.Equal("Test Updated", entity.StringProperty);

            // Act DELETE
            testBll.Delete(entity, true);

            // Assert
            Assert.True(DB.Entry(entity).State == Microsoft.EntityFrameworkCore.EntityState.Detached);
            Assert.Equal(entitiesCount - 1, DB.TestEntities.Count());
        }

        [Fact]
        public void InsertGraph()
        {
            // Arrange
            TestBll testBll = new TestBll(DB);
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

            // Act
            testBll.Save(testEntity);

            // Assert
            Assert.True(DB.Entry(testEntity).State == Microsoft.EntityFrameworkCore.EntityState.Added);
            Assert.True(DB.Entry(testEntity.SecondEntity).State == Microsoft.EntityFrameworkCore.EntityState.Added);

            // Cleanup
            DB.SecondEntities.Remove(testEntity.SecondEntity);
            DB.TestEntities.Remove(testEntity);

            DB.SaveChanges();
        }
    }
}
