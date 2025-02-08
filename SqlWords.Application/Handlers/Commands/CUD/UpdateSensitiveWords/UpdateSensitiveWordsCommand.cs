using MediatR;

namespace SqlWords.Application.Handlers.Commands.CUD.UpdateSensitiveWords
{
	public record UpdateSensitiveWordsCommand(List<(long Id, string NewWord)> Updates) : IRequest<bool>;
}
