using Microsoft.Extensions.Caching.Memory;

using SqlWords.Domain.Entities;
using SqlWords.Infrastructure.Repositories.SensitiveWords;

namespace SqlWords.Service.Caching.Service
{
	public class WordCacheService(ISensitiveWordRepository sensitiveWordRepository, IMemoryCache memoryCache) : ICacheService<string>
	{
		private readonly ISensitiveWordRepository _sensitiveWordRepository = sensitiveWordRepository;
		private readonly IMemoryCache? _memoryCache = memoryCache;
		//move to config
		private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(1);
		private const string CacheKey = "WordCache";
		//move to config

		public async Task<IEnumerable<string>> GetCachedItemsAsync()
		{
			if (!_memoryCache.TryGetValue(CacheKey, out List<string> words))
			{
				words = await LoadWordsFromDataBaseAsync();
				_ = _memoryCache.Set(CacheKey, words, _cacheDuration);
			}
			return words;
		}

		public async Task RefreshCacheAsync()
		{
			_memoryCache.Remove(CacheKey);
			_ = await LoadWordsFromDataBaseAsync();
		}

		private async Task<List<string>?> LoadWordsFromDataBaseAsync()
		{
			IEnumerable<SensitiveWord> sensitiveWords = await _sensitiveWordRepository.GetAllAsync();
			List<string> words = [];

			foreach (SensitiveWord word in sensitiveWords)
			{
				if (!string.IsNullOrWhiteSpace(word.Word))
				{
					words.Add(word.Word);
				}
			}

			_ = _memoryCache.Set(CacheKey, words, _cacheDuration);
			return words;
		}
	}
}
