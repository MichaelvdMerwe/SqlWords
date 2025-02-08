using MediatR;

using SqlWords.Domain.Entities;
using SqlWords.Infrastructure.Repositories;

namespace SqlWords.Application.Handlers.Commands.CUD.AddSensitiveWords
{

	public class AddSensitiveWordsCommandHandler(IRepository<SensitiveWord> repository) : IRequestHandler<AddSensitiveWordsCommand, List<long>>
	{
		private readonly IRepository<SensitiveWord> _repository = repository;

		public async Task<List<long>> Handle(AddSensitiveWordsCommand request, CancellationToken cancellationToken)
		{
			List<SensitiveWord> words = request.Words.ConvertAll(word => new SensitiveWord(word));
			_ = await _repository.AddRangeAsync(words);
			return words.ConvertAll(w => w.Id);
		}
	}
}