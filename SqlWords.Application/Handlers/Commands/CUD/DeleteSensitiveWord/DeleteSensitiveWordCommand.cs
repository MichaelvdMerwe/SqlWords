using MediatR;

namespace SqlWords.Application.Handlers.Commands.CUD.DeleteSensitiveWord
{
	public record DeleteSensitiveWordCommand(long Id) : IRequest<bool>;
}
