namespace SqlWords.Api.Controllers.Dto.SensitiveWord
{
	public class DeleteSensitiveWordsDto
	{
		public required List<long> Ids { get; set; }
	}
}
