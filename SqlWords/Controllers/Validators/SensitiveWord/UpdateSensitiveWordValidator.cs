using FluentValidation;
using SqlWords.Api.Controllers.Dto.SensitiveWords;

namespace SqlWords.Api.Controllers.Validators.SensitiveWord
{
    public class UpdateSensitiveWordValidator : AbstractValidator<UpdateSensitiveWordDto>
    {
        public UpdateSensitiveWordValidator()
        {
            _ = RuleFor(x => x.Id).GreaterThan(0).WithMessage("ID must be greater than zero.");
            _ = RuleFor(x => x.Word).NotEmpty().WithMessage("Word cannot be empty.");
        }
    }
}
