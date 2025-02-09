using FluentValidation;

using SqlWords.Api.Controllers.Dto.SensitiveWord;

namespace SqlWords.Api.Controllers.Validators.SensitiveWord
{
	public class DeleteSensitiveWordsValidator : AbstractValidator<DeleteSensitiveWordsDto>
    {
        public DeleteSensitiveWordsValidator()
        {
            RuleFor(x => x.Ids)
                .NotEmpty().WithMessage("ID list cannot be empty.");
        }
    }
}
