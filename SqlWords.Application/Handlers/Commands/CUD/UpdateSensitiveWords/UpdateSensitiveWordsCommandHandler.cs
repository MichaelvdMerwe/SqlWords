using MediatR;

using SqlWords.Domain.Entities;
using SqlWords.Infrastructure.Repositories;

namespace SqlWords.Application.Handlers.Commands.CUD.UpdateSensitiveWords
{
	public class UpdateSensitiveWordsCommandHandler(IRepository<SensitiveWord> repository) : IRequestHandler<UpdateSensitiveWordsCommand, bool>
	{
		private readonly IRepository<SensitiveWord> _repository = repository;

		public async Task<bool> Handle(UpdateSensitiveWordsCommand request, CancellationToken cancellationToken)
		{
			List<SensitiveWord> wordsToUpdate = [];

			foreach ((long Id, string NewWord) in request.Updates)
			{
				SensitiveWord? existingWord = await _repository.GetAsync(Id);
				if (existingWord == null)
				{
					return false;
				}

				wordsToUpdate.Add(new SensitiveWord(NewWord));
			}

			_ = await _repository.UpdateRangeAsync(wordsToUpdate);
			return true;
		}
	}
}
