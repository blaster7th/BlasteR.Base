namespace BlasteR.Base.Tests
{
    public class SecondBll : BaseBll<SecondEntity>
    {
        public SecondBll(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}