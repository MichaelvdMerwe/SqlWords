using MediatR;

using SqlWords.Domain.Entities;
using SqlWords.Infrastructure.UnitOfWork.Repositories;
using SqlWords.Infrastructure.UnitOfWork.Repositories.SensitiveWords;
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
			List<SensitiveWord> wordsToDelete = [];

			foreach (long id in request.Ids)
			{
				SensitiveWord? word = await _sensitiveWordRepository.GetAsync(id);
				if (word == null)
				{
					return false;
				}

				wordsToDelete.Add(word);
			}

			_ = await _sensitiveWordRepository.DeleteRangeAsync(wordsToDelete);

			await _cacheService.RefreshCacheAsync();
			return true;
		}
	}
}
