using MediatR;

using SqlWords.Domain.Entities;
using SqlWords.Infrastructure.Repositories;
using SqlWords.Infrastructure.Repositories.SensitiveWords;
using SqlWords.Service.Caching.Service;

namespace SqlWords.Application.Handlers.Commands.CUD.UpdateSensitiveWords
{
	public class UpdateSensitiveWordsCommandHandler
	(
		ISensitiveWordRepository sensitiveWordRepository,
		ICacheService<string> cacheService
	) : IRequestHandler<UpdateSensitiveWordsCommand, bool>
	{
		private readonly IRepository<SensitiveWord> _sensitiveWordRepository = sensitiveWordRepository;
		private readonly ICacheService<string> _cacheService = cacheService;

		public async Task<bool> Handle(UpdateSensitiveWordsCommand request, CancellationToken cancellationToken)
		{
			List<SensitiveWord> wordsToUpdate = [];

			foreach ((long Id, string NewWord) in request.Updates)
			{
				SensitiveWord? existingWord = await _sensitiveWordRepository.GetAsync(Id);
				if (existingWord == null)
				{
					return false;
				}

				wordsToUpdate.Add(new SensitiveWord(NewWord));
			}

			_ = await _sensitiveWordRepository.UpdateRangeAsync(wordsToUpdate);
			return true;
		}
	}
}
