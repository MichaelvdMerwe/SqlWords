using FluentValidation;
using SqlWords.Api.Controllers.Dto.Sanitizer;

namespace SqlWords.Api.Controllers.Validators.Sanitizer
{
    public class SanitizeRequestValidator : AbstractValidator<SanitizeRequestDto>
    {
        public SanitizeRequestValidator()
        {
            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message cannot be empty.")
                .MaximumLength(500).WithMessage("Message cannot exceed 500 characters.");
        }
    }
}
