using MediatR;

using SqlWords.Domain.Entities;
using SqlWords.Infrastructure.Repositories.SensitiveWords;
using SqlWords.Service.Sanitizer.Service;

namespace SqlWords.Application.Handlers.Queries.SanitizeMessage
{
	public class SanitizeMessageQueryHandler(ISensitiveWordRepository sensitiveWordRepository, ISanitizerService sanitizerService) : IRequestHandler<SanitizeMessageQuery, string>
	{
		private readonly ISensitiveWordRepository _sensitiveWordRepository = sensitiveWordRepository;
		private readonly ISanitizerService _sanitizerService = sanitizerService;
		public async Task<string> Handle(SanitizeMessageQuery request, CancellationToken cancellationToken)
		{
			IEnumerable<SensitiveWord> sensitiveWords = await _sensitiveWordRepository.GetAllAsync();
			List<string> wordList = [];
			foreach (SensitiveWord sensitiveWord in sensitiveWords)
			{
				wordList.Add(sensitiveWord.Word);
			}
			return _sanitizerService.Sanitize(wordList, request.Message);
		}
	}
}
