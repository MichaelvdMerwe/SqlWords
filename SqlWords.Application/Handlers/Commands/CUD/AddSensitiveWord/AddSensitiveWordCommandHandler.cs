using MediatR;

using SqlWords.Domain.Entities;
using SqlWords.Infrastructure.UnitOfWork.Repositories.SensitiveWords;
using SqlWords.Service.Caching.Service;

namespace SqlWords.Application.Handlers.Commands.CUD.AddSensitiveWord
{
	public class AddSensitiveWordCommandHandler
	(
		ISensitiveWordRepository sensitiveWordRepository,
		ICacheService<string> cacheService
	) : IRequestHandler<AddSensitiveWordCommand, long>
	{
		private readonly ISensitiveWordRepository _sensitiveWordRepository = sensitiveWordRepository;
		private readonly ICacheService<string> _cacheService = cacheService;

		public async Task<long> Handle(AddSensitiveWordCommand request, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(request.Word))
			{
				throw new ArgumentException("Word cannot be empty.", nameof(request));
			}

			SensitiveWord newWord = new(request.Word);
			try
			{
				newWord.Id = await _sensitiveWordRepository.AddAsync(newWord);
				await _cacheService.RefreshCacheAsync();
				return newWord.Id;
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Failed to add the sensitive word.", ex);
			}
		}
	}
}
