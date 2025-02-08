using MediatR;

namespace SqlWords.Application.Handlers.Queries.SanitizeMessage
{
	public record SanitizeMessageQuery(string Message) : IRequest<string>;
}
