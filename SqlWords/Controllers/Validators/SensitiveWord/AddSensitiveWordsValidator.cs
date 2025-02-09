using FluentValidation;

using SqlWords.Api.Controllers.Dto.SensitiveWords;

namespace SqlWords.Api.Controllers.Validators.SensitiveWord
{
	public class AddSensitiveWordsValidator : AbstractValidator<AddSensitiveWordsDto>
	{
		public AddSensitiveWordsValidator()
		{
			_ = RuleFor(x => x.Words)
				.NotEmpty().WithMessage("Words list cannot be empty.");
		}
	}
}
