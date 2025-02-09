namespace SqlWords.Api.Controllers.Dto.SensitiveWords
{
    public class AddSensitiveWordsDto
    {
        public required List<string> Words { get; set; }
    }
}
