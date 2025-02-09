using MediatR;

using SqlWords.Domain.Entities;
using SqlWords.Infrastructure.UnitOfWork.Repositories.SensitiveWords;

namespace SqlWords.Application.Handlers.Queries.GetAllSensitiveWords
{
    public class GetAllSensitiveWordsQueryHandler(ISensitiveWordRepository sensitiveWordRepository)
		: IRequestHandler<GetAllSensitiveWordsQuery, IEnumerable<SensitiveWord>>
	{
		private readonly ISensitiveWordRepository _sensitiveWordRepository = sensitiveWordRepository;
		public async Task<IEnumerable<SensitiveWord>> Handle(GetAllSensitiveWordsQuery request, CancellationToken cancellationToken)
		{
			return await _sensitiveWordRepository.GetAllAsync();
		}
	}
}
