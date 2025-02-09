using MediatR;

using SqlWords.Domain.Entities;
using SqlWords.Infrastructure.UnitOfWork.Repositories.SensitiveWords;

namespace SqlWords.Application.Handlers.Queries.GetSensitiveWordById
{
    public class GetSensitiveWordByIdQueryHandler(ISensitiveWordRepository sensitiveWordRepository)
		: IRequestHandler<GetSensitiveWordByIdQuery, SensitiveWord?>
	{
		private readonly ISensitiveWordRepository _sensitiveWordRepository = sensitiveWordRepository;
		public async Task<SensitiveWord?> Handle(GetSensitiveWordByIdQuery request, CancellationToken cancellationToken)
		{
			return await _sensitiveWordRepository.GetAsync(request.Id);
		}
	}
}
