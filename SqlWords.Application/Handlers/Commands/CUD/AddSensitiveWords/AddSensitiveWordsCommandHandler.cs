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
	) : IRequestHandler<AddSensitiveWordsCommand, int>
	{
		private readonly IRepository<SensitiveWord> _sensitiveWordRepository = sensitiveWordRepository;
		private readonly ICacheService<string> _cacheService = cacheService;

		public async Task<int> Handle(AddSensitiveWordsCommand request, CancellationToken cancellationToken)
		{
			if (request.Words is null || request.Words.Count == 0)
			{
				throw new ArgumentException("Words list cannot be empty.", nameof(request));
			}

			// Ensure no empty words & remove duplicates (case-insensitive)
			List<SensitiveWord> words = request.Words
				.Where(word => !string.IsNullOrWhiteSpace(word)) // Remove empty/null words
				.Distinct(StringComparer.OrdinalIgnoreCase) // Remove duplicates
				.Select(word => new SensitiveWord(word))
				.ToList();

			if (words.Count == 0)
			{
				throw new ArgumentException("All words provided were empty or duplicates.");
			}

			// Insert words and get affected rows count
			int affectedRows = await _sensitiveWordRepository.AddRangeAsync(words);

			if (affectedRows != words.Count)
			{
				throw new InvalidOperationException("Mismatch in affected rows count. Some inserts may have failed.");
			}

			await _cacheService.RefreshCacheAsync();
			return affectedRows;
		}
	}
}