using MediatR;

using SqlWords.Domain.Entities;
using SqlWords.Infrastructure.Repositories;
using SqlWords.Infrastructure.Repositories.SensitiveWords;
using SqlWords.Service.Caching.Service;

namespace SqlWords.Application.Handlers.Commands.CUD.DeleteSensitiveWords
{
	public class DeleteSensitiveWordsCommandHandler
	(
		ISensitiveWordRepository sensitiveWordRepository,
		ICacheService<string> cacheService
	) : IRequestHandler<DeleteSensitiveWordsCommand, bool>
	{
		private readonly IRepository<SensitiveWord> _sensitiveWordRepository = sensitiveWordRepository;
		private readonly ICacheService<string> _cacheService = cacheService;

		public async Task<bool> Handle(DeleteSensitiveWordsCommand request, CancellationToken cancellationToken)
		{
			List<SensitiveWord> words = [];

			foreach (long id in request.Ids)
			{
				SensitiveWord? word = await _sensitiveWordRepository.GetAsync(id);
				if (word == null)
				{
					return false;
				}

				words.Add(word);
			}

			_ = await _sensitiveWordRepository.DeleteRangeAsync(words);

			await _cacheService.RefreshCacheAsync();
			return true;
		}
	}
}
