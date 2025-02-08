using MediatR;

using SqlWords.Domain.Entities;

namespace SqlWords.Application.Handlers.Queries.GetSensitiveWordById
{
	public record GetSensitiveWordByIdQuery(long Id) : IRequest<SensitiveWord?>;
}
