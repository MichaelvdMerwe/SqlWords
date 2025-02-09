using MediatR;

namespace SqlWords.Application.Handlers.Commands.CUD.AddSensitiveWords
{
	public record AddSensitiveWordsCommand(List<string> Words) : IRequest<int>;
}