using SqlWords.Domain.Entities;
using SqlWords.Infrastructure.UnitOfWork.Repositories.SensitiveWords;
using SqlWords.Service.Caching.Service;

namespace SqlWords.Service.Caching.Tests.Service
{
	public class WordCacheServiceTests
	{
		private Mock<ISensitiveWordRepository> _mockRepository;
		private IMemoryCache _memoryCache;
		private Mock<IConfiguration> _mockConfiguration;
		private Mock<ILogger<WordCacheService>> _mockLogger;
		private WordCacheService _wordCacheService;

		[SetUp]
		public void Setup()
		{
			_mockRepository = new Mock<ISensitiveWordRepository>();
			_memoryCache = new MemoryCache(new MemoryCacheOptions());
			_mockConfiguration = new Mock<IConfiguration>();
			_mockLogger = new Mock<ILogger<WordCacheService>>();

			Mock<IConfigurationSection> mockConfigSection = new();
			_ = mockConfigSection.Setup(s => s.Value).Returns("60");

			_ = _mockConfiguration
				.Setup(c => c.GetSection("CacheDurationMinutes"))
				.Returns(mockConfigSection.Object);

			_wordCacheService = new WordCacheService(
				_mockRepository.Object,
				_memoryCache,
				_mockConfiguration.Object,
				_mockLogger.Object
			);
		}


		[Test]
		public async Task GetCachedItemsAsync_CacheHit_ReturnsCachedWords()
		{
			// Arrange
			List<string> cachedWords = ["SELECT", "DELETE"];
			_ = _memoryCache.Set("WordCache", cachedWords, TimeSpan.FromMinutes(60));

			// Act
			IEnumerable<string> result = await _wordCacheService.GetCachedItemsAsync();

			// Assert
			_ = result.Should().NotBeNull();
			_ = result.Should().BeEquivalentTo(cachedWords);
			_mockRepository.Verify(repo => repo.GetAllAsync(), Times.Never); // Ensure DB was not called
		}

		[Test]
		public async Task GetCachedItemsAsync_CacheMiss_LoadsFromDatabase()
		{
			// Arrange
			List<SensitiveWord> dbWords = [new("UPDATE"), new("DROP")];
			_ = _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(dbWords);

			// Act
			IEnumerable<string> result = await _wordCacheService.GetCachedItemsAsync();

			// Assert
			_ = result.Should().BeEquivalentTo(new List<string> { "UPDATE", "DROP" });
			_mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once); // Ensure DB was called
		}

		[Test]
		public async Task RefreshCacheAsync_ClearsAndReloadsCache()
		{
			// Arrange
			List<SensitiveWord> dbWords = [new("INSERT"), new("TRUNCATE")];
			_ = _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(dbWords);

			// Act
			await _wordCacheService.RefreshCacheAsync();
			IEnumerable<string> result = await _wordCacheService.GetCachedItemsAsync();

			// Assert
			_ = result.Should().BeEquivalentTo(new List<string> { "INSERT", "TRUNCATE" });
			_mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once); // Ensure DB was queried
		}

		[Test]
		public void GetCachedItemsAsync_DatabaseFailure_ThrowsException()
		{
			// Arrange
			_ = _mockRepository.Setup(repo => repo.GetAllAsync()).ThrowsAsync(new Exception("DB Connection Failed"));

			// Act
			Func<Task> act = async () => await _wordCacheService.GetCachedItemsAsync();

			// Assert
			_ = act.Should().ThrowAsync<ApplicationException>()
				.WithMessage("Cache retrieval failed.")
				.WithMessage("DB Connection Failed");

			_mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
		}

		[Test]
		public async Task RefreshCacheAsync_DatabaseFailure_HandlesExceptionGracefully()
		{
			// Arrange
			_ = _mockRepository.Setup(repo => repo.GetAllAsync()).ThrowsAsync(new Exception("DB Failure"));

			// Act
			Func<Task> act = async () => await _wordCacheService.RefreshCacheAsync();

			// Assert
			_ = await act.Should().NotThrowAsync();
			_mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
		}
	}
}
