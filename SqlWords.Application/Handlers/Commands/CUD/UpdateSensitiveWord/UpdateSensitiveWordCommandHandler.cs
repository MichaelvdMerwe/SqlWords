using MediatR;

using SqlWords.Domain.Entities;
using SqlWords.Infrastructure.Repositories;

namespace SqlWords.Application.Handlers.Commands.CUD.UpdateSensitiveWord
{
	public class UpdateSensitiveWordCommandHandler(IRepository<SensitiveWord> repository) : IRequestHandler<UpdateSensitiveWordCommand, bool>
	{
		private readonly IRepository<SensitiveWord> _repository = repository;

		public async Task<bool> Handle(UpdateSensitiveWordCommand request, CancellationToken cancellationToken)
		{
			SensitiveWord? existingWord = await _repository.GetAsync(request.Id);
			if (existingWord == null)
			{
				return false;
			}

			SensitiveWord updatedWord = new(request.NewWord);
			_ = await _repository.UpdateAsync(updatedWord);
			return true;
		}
	}
}
