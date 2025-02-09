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
			_ = await _sensitiveWordRepository.AddRangeAsync(words);

			await _cacheService.RefreshCacheAsync();
			return words.ConvertAll(w => w.Id);
		}
	}
}