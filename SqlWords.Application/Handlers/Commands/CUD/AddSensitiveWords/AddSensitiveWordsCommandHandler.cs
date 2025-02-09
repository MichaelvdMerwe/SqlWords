using MediatR;

using SqlWords.Domain.Entities;
using SqlWords.Infrastructure.UnitOfWork.Repositories;
using SqlWords.Infrastructure.UnitOfWork.Repositories.SensitiveWords;
using SqlWords.Service.Caching.Service;

namespace SqlWords.Application.Handlers.Commands.CUD.AddSensitiveWords
{

	public class AddSensitiveWordsCommandHandler(
		ISensitiveWordRepository sensitiveWordRepository,
		ICacheService<string> cacheService
	) : IRequestHandler<AddSensitiveWordsCommand, List<long>>
	{
		private readonly IRepository<SensitiveWord> _sensitiveWordRepository = sensitiveWordRepository;
		private readonly ICacheService<string> _cacheService = cacheService;

		public async Task<List<long>> Handle(AddSensitiveWordsCommand request, CancellationToken cancellationToken)
		{
			List<SensitiveWord> words = request.Words.ConvertAll(word => new SensitiveWord(word));
			List<long> insertedIds = await _sensitiveWordRepository.AddRangeAsync(words);

			for (int i = 0; i < words.Count; i++)
			{
				words[i].Id = insertedIds[i];
			}

			await _cacheService.RefreshCacheAsync();
			return insertedIds;
		}
	}
}