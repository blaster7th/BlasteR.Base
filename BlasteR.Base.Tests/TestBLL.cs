namespace BlasteR.Base.Tests
{
    public class TestBll : BaseBll<TestEntity, TestContext>
    {
        public TestBll(TestContext db) : base(db)
        {
        }
    }
}