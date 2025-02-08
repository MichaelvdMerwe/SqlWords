using MediatR;

namespace SqlWords.Application.Handlers.Commands.CUD.UpdateSensitiveWord
{
	public record UpdateSensitiveWordCommand(long Id, string NewWord) : IRequest<bool>;
}
