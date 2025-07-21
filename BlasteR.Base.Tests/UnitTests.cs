using System.Linq;
using Xunit;

namespace BlasteR.Base.Tests
{
    public class UnitTests : IClassFixture<TestFixture>
    {
        public IUnitOfWork UnitOfWork { get; set; }
        public UnitTests(TestFixture fixture)
        {
            UnitOfWork = fixture.UnitOfWork;
        }

        [Fact]
        public void CRUD()
        {
            // Arrange
            FirstBll firstBll = new FirstBll(UnitOfWork);
            var entity = new FirstEntity()
            {
                IntValue = 1,
                StringValue = "Test"
            };

            // Act CREATE
            firstBll.Save(entity);

            // Assert
            int entitiesCount = firstBll.GetAll().Count();
            Assert.NotEqual(0, entity.Id);
            Assert.NotEqual(0, entitiesCount);

            // Act READ
            entity = firstBll.GetById(entity.Id);

            // Assert
            Assert.NotNull(entity);

            // Act UPDATE
            entity.StringValue = "Test Updated";
            firstBll.Save(entity);

            // Assert
            entity = firstBll.GetById(entity.Id);
            Assert.Equal("Test Updated", entity.StringValue);

            // Act DELETE
            firstBll.Delete(entity);

            // Assert
            Assert.Equal(entitiesCount - 1, firstBll.GetAll().Count());
        }

        [Fact]
        public void InsertParentChild()
        {
            // Arrange
            FirstBll firstBll = new FirstBll(UnitOfWork);
            SecondBll secondBll = new SecondBll(UnitOfWork);
            FirstEntity firstEntity = new FirstEntity()
            {
                IntValue = 1,
                StringValue = "Test",
                SecondEntity = new SecondEntity()
                {
                    IntValue = 2,
                    StringValue = "Second"
                }
            };

            // Act
            firstBll.Save(firstEntity);

            // Assert
            Assert.NotEqual(0, firstEntity.Id);
            Assert.NotEqual(0, firstEntity.SecondEntity.Id);
            Assert.Equal(firstEntity.Id, firstEntity.SecondEntity.FirstEntityId);

            // Cleanup
            secondBll.Delete(firstEntity.SecondEntity);
            firstBll.Delete(firstEntity);
        }

        [Fact]
        public void InsertParentChildReverse()
        {
            // Arrange
            FirstBll firstBll = new FirstBll(UnitOfWork);
            SecondBll secondBll = new SecondBll(UnitOfWork);
            SecondEntity secondEntity = new SecondEntity()
            {
                IntValue = 2,
                StringValue = "Second",
                FirstEntity = new FirstEntity()
                {
                    IntValue = 1,
                    StringValue = "Test"
                }
            };

            // Act
            secondBll.Save(secondEntity);

            // Assert
            Assert.NotEqual(0, secondEntity.Id);
            Assert.NotEqual(0, secondEntity.FirstEntity.Id);
            Assert.Equal(secondEntity.FirstEntity.Id, secondEntity.FirstEntityId);

            // Cleanup
            secondBll.Delete(secondEntity);
            firstBll.Delete(secondEntity.FirstEntity);
        }

        [Fact]
        public void SoftDelete()
        {
            // Arrange
            SoftDeletableTestBLL softDeletableTestBLL = new SoftDeletableTestBLL(UnitOfWork);
            SoftDeletableTestEntity softDeletableTestEntity = new SoftDeletableTestEntity()
            {
                IntValue = 1,
                StringValue = "Test",
            };

            softDeletableTestBLL.Save(softDeletableTestEntity);

            // Act Soft Delete
            softDeletableTestBLL.Delete(softDeletableTestEntity);

            // Assert
            Assert.DoesNotContain(softDeletableTestEntity.Id, softDeletableTestBLL.GetAll(false).Select(x => x.Id));
            Assert.Contains(softDeletableTestEntity.Id, softDeletableTestBLL.GetAll(true).Select(x => x.Id));

            // Act Hard Delete
            softDeletableTestBLL.Delete(softDeletableTestEntity, true);

            // Assert
            Assert.DoesNotContain(softDeletableTestEntity.Id, softDeletableTestBLL.GetAll(true).Select(x => x.Id));
        }
    }
}
