using MediatR;

using SqlWords.Domain.Entities;
using SqlWords.Infrastructure.UnitOfWork.Repositories;
using SqlWords.Infrastructure.UnitOfWork.Repositories.SensitiveWords;
using SqlWords.Service.Caching.Service;

namespace SqlWords.Application.Handlers.Commands.CUD.UpdateSensitiveWord
{
    public class UpdateSensitiveWordCommandHandler
	(
		ISensitiveWordRepository sensitiveWordRepository,
		ICacheService<string> cacheService
	) : IRequestHandler<UpdateSensitiveWordCommand, bool>
	{
		private readonly IRepository<SensitiveWord> _sensitiveWordRepository = sensitiveWordRepository;
		private readonly ICacheService<string> _cacheService = cacheService;

		public async Task<bool> Handle(UpdateSensitiveWordCommand request, CancellationToken cancellationToken)
		{
			SensitiveWord? existingWord = await _sensitiveWordRepository.GetAsync(request.Id);
			if (existingWord == null)
			{
				return false;
			}

			SensitiveWord updatedWord = new(request.NewWord);
			_ = await _sensitiveWordRepository.UpdateAsync(updatedWord);


			await _cacheService.RefreshCacheAsync();
			return true;
		}
	}
}
