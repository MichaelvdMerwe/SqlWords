using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using SqlWords.Domain.Entities;
using SqlWords.Infrastructure.UnitOfWork.Repositories.SensitiveWords;

namespace SqlWords.Service.Caching.Service
{
	public class WordCacheService
	(
		ISensitiveWordRepository sensitiveWordRepository,
		IMemoryCache memoryCache,
		IConfiguration configuration,
		ILogger<WordCacheService> logger
	) : ICacheService<string>
	{
		private readonly ISensitiveWordRepository _sensitiveWordRepository = sensitiveWordRepository ?? throw new ArgumentNullException(nameof(sensitiveWordRepository));
		private readonly IMemoryCache _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
		private readonly ILogger<WordCacheService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
		private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(configuration.GetValue("CacheDurationMinutes", 60));
		private const string CacheKey = "WordCache";

		public async Task<IEnumerable<string>> GetCachedItemsAsync()
		{
			try
			{
				if (!_memoryCache.TryGetValue(CacheKey, out List<string>? words) || words is null)
				{
					_logger.LogInformation("Cache miss. Loading words from database.");
					words = await LoadWordsFromDatabaseAsync();
					_ = _memoryCache.Set(CacheKey, words, _cacheDuration);
				}
				else
				{
					_logger.LogInformation("Cache hit. Returning cached words.");
				}
				return words;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to retrieve cached words.");
				throw new ApplicationException("Cache retrieval failed.", ex);
			}
		}

		public async Task RefreshCacheAsync()
		{
			try
			{
				_logger.LogInformation("Refreshing cache...");
				_memoryCache.Remove(CacheKey);
				_ = await LoadWordsFromDatabaseAsync();
				_logger.LogInformation("Cache successfully refreshed.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to refresh cache.");
			}
		}

		private async Task<List<string>> LoadWordsFromDatabaseAsync()
		{
			try
			{
				_logger.LogInformation("Loading words from database...");
				IEnumerable<SensitiveWord> sensitiveWords = await _sensitiveWordRepository.GetAllAsync();

				List<string> words = sensitiveWords
					.Where(word => !string.IsNullOrWhiteSpace(word.Word))
					.Select(word => word.Word)
					.ToList();

				_ = _memoryCache.Set(CacheKey, words, _cacheDuration);
				_logger.LogInformation("Successfully loaded and cached {Count} words from database.", words.Count);

				return words;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to load words from database.");
				throw new ApplicationException("Database query failed while loading word cache.", ex);
			}
		}
	}
}
