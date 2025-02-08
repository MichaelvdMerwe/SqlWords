using MediatR;

namespace SqlWords.Application.Handlers.Commands.CUD.DeleteSensitiveWords
{
	public record DeleteSensitiveWordsCommand(List<long> Ids) : IRequest<bool>;
}
