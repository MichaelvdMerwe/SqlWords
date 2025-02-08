namespace SqlWords.Service.Caching.Service
{
	public interface ICacheService<T>
	{
		Task<IEnumerable<T>> GetCachedItemsAsync();
		Task RefreshCacheAsync();
	}
}
