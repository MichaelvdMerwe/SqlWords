using MediatR;

using SqlWords.Domain.Entities;
using SqlWords.Infrastructure.Repositories;

namespace SqlWords.Application.Handlers.Commands.CUD.DeleteSensitiveWords
{
	public class DeleteSensitiveWordsCommandHandler(IRepository<SensitiveWord> repository) : IRequestHandler<DeleteSensitiveWordsCommand, bool>
	{
		private readonly IRepository<SensitiveWord> _repository = repository;

		public async Task<bool> Handle(DeleteSensitiveWordsCommand request, CancellationToken cancellationToken)
		{
			List<SensitiveWord> words = [];

			foreach (long id in request.Ids)
			{
				SensitiveWord? word = await _repository.GetAsync(id);
				if (word == null)
				{
					return false;
				}

				words.Add(word);
			}

			_ = await _repository.DeleteRangeAsync(words);
			return true;
		}
	}
}
