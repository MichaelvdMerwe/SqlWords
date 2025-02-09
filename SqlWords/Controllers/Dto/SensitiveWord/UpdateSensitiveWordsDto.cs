namespace SqlWords.Api.Controllers.Dto.SensitiveWord
{
	public class UpdateSensitiveWordsDto
	{
		public required List<(long Id, string Word)> Words { get; set; }
	}
}
