using MediatR;

using SqlWords.Domain.Entities;

namespace SqlWords.Application.Handlers.Queries.GetAllSensitiveWords
{
	public record GetAllSensitiveWordsQuery() : IRequest<IEnumerable<SensitiveWord>> { }

}
