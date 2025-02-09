using FluentValidation;
using SqlWords.Api.Controllers.Dto.SensitiveWords;

namespace SqlWords.Api.Controllers.Validators.SensitiveWord
{
    public class UpdateSensitiveWordsValidator : AbstractValidator<UpdateSensitiveWordsDto>
    {
        public UpdateSensitiveWordsValidator()
        {
            RuleFor(x => x.Words)
                .NotEmpty().WithMessage("Words list cannot be empty.");
        }
    }
}
