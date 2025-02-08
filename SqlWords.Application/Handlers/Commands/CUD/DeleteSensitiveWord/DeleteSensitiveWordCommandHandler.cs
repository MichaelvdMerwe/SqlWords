using MediatR;

using SqlWords.Domain.Entities;
using SqlWords.Infrastructure.Repositories;

namespace SqlWords.Application.Handlers.Commands.CUD.DeleteSensitiveWord
{
	public record DeleteSensitiveWordCommand(long Id) : IRequest<bool>;

	public class DeleteSensitiveWordCommandHandler(IRepository<SensitiveWord> repository) : IRequestHandler<DeleteSensitiveWordCommand, bool>
	{
		private readonly IRepository<SensitiveWord> _repository = repository;

		public async Task<bool> Handle(DeleteSensitiveWordCommand request, CancellationToken cancellationToken)
		{
			SensitiveWord? word = await _repository.GetAsync(request.Id);
			if (word == null)
			{
				return false;
			}

			_ = await _repository.DeleteAsync(request.Id);
			return true;
		}
	}
}
