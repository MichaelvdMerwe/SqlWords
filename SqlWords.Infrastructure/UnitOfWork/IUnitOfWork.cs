namespace SqlWords.Infrastructure.UnitOfWork
{
	public interface IUnitOfWork
	{
		public void Complete();
		public void Rollback();
	}
}
