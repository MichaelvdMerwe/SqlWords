using MediatR;

using SqlWords.Service.Caching.Service;
using SqlWords.Service.Sanitizer.Service;

namespace SqlWords.Application.Handlers.Queries.SanitizeMessage
{
	public class SanitizeMessageQueryHandler
	(
		ICacheService<string> WordCacheService,
		ISanitizerService sanitizerService
	) : IRequestHandler<SanitizeMessageQuery, string>
	{
		private readonly ICacheService<string> wordCacheService = WordCacheService;
		private readonly ISanitizerService _sanitizerService = sanitizerService;
		public async Task<string> Handle(SanitizeMessageQuery request, CancellationToken cancellationToken)
		{
			IEnumerable<string> wordList = await wordCacheService.GetCachedItemsAsync();
			return _sanitizerService.Sanitize(wordList, request.Message);
		}
	}
}
