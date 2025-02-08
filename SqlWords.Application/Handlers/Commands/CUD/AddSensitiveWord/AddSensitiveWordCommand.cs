using MediatR;

namespace SqlWords.Application.Handlers.Commands.CUD.AddSensitiveWord
{
	public record AddSensitiveWordCommand(string Word) : IRequest<long>;
}
