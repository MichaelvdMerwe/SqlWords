namespace SqlWords.Domain.Entities
{
	public class SensitiveWord(string word)
	{
		public long Id { get; set; }
		public string Word { get; set; } = word ?? throw new ArgumentNullException(nameof(word));
		public DateTime CreatedAt { get; set; } = DateTime.Now;
	}
}