namespace BlasteR.Base.Tests
{
    public class SoftDeletableTestBLL : BaseBll<SoftDeletableTestEntity>
    {
        public SoftDeletableTestBLL(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}