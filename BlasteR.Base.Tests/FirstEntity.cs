namespace BlasteR.Base.Tests
{
    public class FirstEntity : BaseEntity
    {
        public int IntValue { get; set; }
        public string StringValue { get; set; }
        
        public virtual SecondEntity SecondEntity { get; set; }
    }
}