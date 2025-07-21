namespace BlasteR.Base.Tests
{
    public class FirstBll : BaseBll<FirstEntity>
    {
        public FirstBll(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}