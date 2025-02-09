using MediatR;

using Microsoft.Extensions.Logging;

using SqlWords.Service.Caching.Service;
using SqlWords.Service.Sanitizer.Service;

namespace SqlWords.Application.Handlers.Queries.SanitizeMessage
{
	public class SanitizeMessageQueryHandler
	(
		ICacheService<string> wordCacheService,
		ISanitizerService sanitizerService,
		ILogger<SanitizeMessageQueryHandler> logger
	) : IRequestHandler<SanitizeMessageQuery, string>
	{
		private readonly ICacheService<string> _wordCacheService = wordCacheService;
		private readonly ISanitizerService _sanitizerService = sanitizerService;
		private readonly ILogger<SanitizeMessageQueryHandler> _logger = logger;
		public async Task<string> Handle(SanitizeMessageQuery request, CancellationToken cancellationToken)
		{
			try
			{
				_logger.LogInformation("Sanitizing message: {Message}", request.Message);

				IEnumerable<string> wordList = await _wordCacheService.GetCachedItemsAsync();
				if (!wordList.Any())
				{
					_logger.LogWarning("Cache is empty! No words available for sanitization.");
					return request.Message; // Return unmodified message if cache is empty
				}

				string sanitizedMessage = _sanitizerService.Sanitize(wordList, request.Message);
				_logger.LogInformation("Sanitization complete.");

				return sanitizedMessage;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while sanitizing message: {Message}", request.Message);
				throw new ApplicationException("An error occurred during message sanitization.", ex);
			}
		}
	}
}
