namespace BlasteR.Base.Tests
{
    public class TestEntity : BaseEntity
    {
        public int IntProperty { get; set; }
        public string StringProperty { get; set; }
        
        public virtual SecondEntity SecondEntity { get; set; }
    }
}