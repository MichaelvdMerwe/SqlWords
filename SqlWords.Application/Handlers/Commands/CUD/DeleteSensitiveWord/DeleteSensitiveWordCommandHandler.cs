using MediatR;

using SqlWords.Domain.Entities;
using SqlWords.Infrastructure.UnitOfWork.Repositories;
using SqlWords.Infrastructure.UnitOfWork.Repositories.SensitiveWords;
using SqlWords.Service.Caching.Service;

namespace SqlWords.Application.Handlers.Commands.CUD.DeleteSensitiveWord
{

    public class DeleteSensitiveWordCommandHandler
	(
		ISensitiveWordRepository sensitiveWordRepository,
		ICacheService<string> cacheService
	) : IRequestHandler<DeleteSensitiveWordCommand, bool>
	{
		private readonly IRepository<SensitiveWord> _sensitiveWordRepository = sensitiveWordRepository;
		private readonly ICacheService<string> _cacheService = cacheService;

		public async Task<bool> Handle(DeleteSensitiveWordCommand request, CancellationToken cancellationToken)
		{
			SensitiveWord? word = await _sensitiveWordRepository.GetAsync(request.Id);
			if (word == null)
			{
				return false;
			}

			_ = await _sensitiveWordRepository.DeleteAsync(request.Id);

			await _cacheService.RefreshCacheAsync();
			return true;
		}
	}
}
