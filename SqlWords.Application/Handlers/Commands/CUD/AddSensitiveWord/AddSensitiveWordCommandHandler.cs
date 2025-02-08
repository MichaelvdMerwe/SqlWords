using MediatR;

using SqlWords.Domain.Entities;
using SqlWords.Infrastructure.Repositories.SensitiveWords;

namespace SqlWords.Application.Handlers.Commands.CUD.AddSensitiveWord
{
	public class AddSensitiveWordCommandHandler(ISensitiveWordRepository sensitiveWordRepository)
		: IRequestHandler<AddSensitiveWordCommand, long>
	{
		private readonly ISensitiveWordRepository _sensitiveWordRepository = sensitiveWordRepository;

		public async Task<long> Handle(AddSensitiveWordCommand request, CancellationToken cancellationToken)
		{
			SensitiveWord newWord = new(request.Word);
			_ = await _sensitiveWordRepository.AddAsync(newWord);
			return newWord.Id;
		}
	}
}
