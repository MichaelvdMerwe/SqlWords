namespace SqlWords.Infrastructure.UnitOfWork.Repositories
{
	public interface IRepository<T>
	{
		Task<IEnumerable<T>> GetAllAsync();

		Task<T?> GetAsync(long id);

		Task<long> AddAsync(T entity);

		Task<int> AddRangeAsync(IEnumerable<T> entities);

		Task<int> UpdateAsync(T entity);

		Task<int> UpdateRangeAsync(IEnumerable<T> entities);

		Task<int> DeleteAsync(long id);

		Task<int> DeleteRangeAsync(IEnumerable<T> entities);
	}
}