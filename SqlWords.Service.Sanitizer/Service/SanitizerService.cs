
using System.Text.RegularExpressions;

namespace SqlWords.Service.Sanitizer.Service
{
	public class SanitizerService : ISanitizerService
	{
		public string Sanitize(IEnumerable<string> sensitiveWords, string message)
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				return message;
			}

			foreach (string word in sensitiveWords)
			{
				string pattern = $@"\b{Regex.Escape(word)}\b";
				message = Regex.Replace(message, pattern, "****", RegexOptions.IgnoreCase);
			}

			return message;
		}
	}
}
