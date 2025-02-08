using MediatR;

using SqlWords.Domain.Entities;

namespace SqlWords.Application.Handlers.Queries.GetSensitiveWordByWord
{
	public record GetSensitiveWordByWordQuery(string Word) : IRequest<SensitiveWord>;
}
