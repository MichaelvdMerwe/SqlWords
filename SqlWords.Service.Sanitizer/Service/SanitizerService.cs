using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

namespace SqlWords.Service.Sanitizer.Service
{
	public class SanitizerService(ILogger<SanitizerService> logger) : ISanitizerService
	{
		private readonly ILogger<SanitizerService> _logger = logger;

		//not entirely happy with this sanitizer. Will need to look at performance, there is also an edge case that i just cant seem to crack with the rejex
		//try other algorithms
		public string Sanitize(IEnumerable<string> sensitiveWords, string message)
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				_logger.LogWarning("Sanitize method received an empty or null message.");
				return message;
			}

			try
			{
				_logger.LogInformation("Sanitizing message.");

				string sanitizedMessage = message;

				foreach (string word in sensitiveWords)
				{
					string pattern = $@"\b{Regex.Escape(word)}\b";

					sanitizedMessage = Regex.Replace(
						sanitizedMessage,
						pattern,
						match => new string('*', match.Length),
						RegexOptions.IgnoreCase | RegexOptions.CultureInvariant
					);
				}

				_logger.LogInformation("Message sanitized successfully.");
				return sanitizedMessage;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while sanitizing message.");
				return message;
			}
		}
	}
}
