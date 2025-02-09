using FluentValidation;

using SqlWords.Api.Controllers.Dto.SensitiveWord;

namespace SqlWords.Api.Controllers.Validators.SensitiveWord
{
	public class AddSensitiveWordValidator : AbstractValidator<AddSensitiveWordDto>
    {
        public AddSensitiveWordValidator()
        {
            _ = RuleFor(x => x.Word)
                .NotEmpty().WithMessage("Word cannot be empty.")
                .MaximumLength(100).WithMessage("Word cannot exceed 100 characters.");
        }
    }
}
