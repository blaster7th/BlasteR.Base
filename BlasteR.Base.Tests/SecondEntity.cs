namespace BlasteR.Base.Tests
{
    public class SecondEntity : BaseEntity
    {
        public int IntValue { get; set; }
        public string StringValue { get; set; }

        public int TestEntityId { get; set; }
        public virtual TestEntity TestEntity { get; set; }
    }
}