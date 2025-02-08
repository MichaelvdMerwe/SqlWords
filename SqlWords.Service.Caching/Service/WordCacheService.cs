using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

using SqlWords.Domain.Entities;
using SqlWords.Infrastructure.Repositories.SensitiveWords;

namespace SqlWords.Service.Caching.Service
{
	public class WordCacheService
	(
		ISensitiveWordRepository sensitiveWordRepository,
		IMemoryCache memoryCache,
		IConfiguration configuration
	) : ICacheService<string>
	{
		private readonly ISensitiveWordRepository _sensitiveWordRepository = sensitiveWordRepository ?? throw new ArgumentNullException(nameof(sensitiveWordRepository));
		private readonly IMemoryCache _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
		private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(configuration.GetValue("CacheDurationMinutes", 60));
		private const string CacheKey = "WordCache";

		public async Task<IEnumerable<string>> GetCachedItemsAsync()
		{
			if (!_memoryCache.TryGetValue(CacheKey, out List<string> words))
			{
				words = await LoadWordsFromDatabaseAsync();
				_ = _memoryCache.Set(CacheKey, words, _cacheDuration);
			}
			return words;
		}

		public async Task RefreshCacheAsync()
		{
			_memoryCache.Remove(CacheKey);
			_ = await LoadWordsFromDatabaseAsync();
		}

		private async Task<List<string>> LoadWordsFromDatabaseAsync()
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
