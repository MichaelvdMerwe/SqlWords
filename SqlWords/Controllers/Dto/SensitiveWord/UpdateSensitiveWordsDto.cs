namespace SqlWords.Api.Controllers.Dto.SensitiveWords
{
    public class UpdateSensitiveWordsDto
    {
        public List<(long Id, string Word)> Words { get; set; }
    }
}
