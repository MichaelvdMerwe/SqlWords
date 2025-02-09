using MediatR;

using SqlWords.Domain.Entities;
using SqlWords.Infrastructure.UnitOfWork.Repositories.SensitiveWords;

namespace SqlWords.Application.Handlers.Queries.GetSensitiveWordByWord
{
    public class GetSensitiveWordByWordQueryHandler(ISensitiveWordRepository sensitiveWordRepository) : IRequestHandler<GetSensitiveWordByWordQuery, SensitiveWord?>
	{
		private readonly ISensitiveWordRepository _sensitiveWordRepository = sensitiveWordRepository;
		public async Task<SensitiveWord?> Handle(GetSensitiveWordByWordQuery request, CancellationToken cancellationToken)
		{
			return await _sensitiveWordRepository.GetByWordAsync(request.Word);
		}
	}
}
