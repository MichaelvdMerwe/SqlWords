namespace SqlWords.Api.Controllers.Dto.SensitiveWords
{
    public class UpdateSensitiveWordDto
    {
        public long Id { get; set; }
        public required string Word { get; set; }
    }
}
