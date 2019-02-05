namespace BlasteR.Base.Tests
{
    public class TestBLL : BaseBLL<TestEntity, TestContext>
    {
        public TestBLL(TestContext db) : base(db)
        {
        }
    }
}