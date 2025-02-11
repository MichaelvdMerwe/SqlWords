﻿namespace SqlWords.Domain.Entities
{
	public class SensitiveWord(string word) : Entity
	{
		public SensitiveWord() : this(string.Empty) { }
		public string Word { get; set; } = word ?? throw new ArgumentNullException(nameof(word));
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}