using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

namespace SqlWords.Service.Sanitizer.Service
{
	public class SanitizerService(ILogger<SanitizerService> logger) : ISanitizerService
	{
		private readonly ILogger<SanitizerService> _logger = logger;

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

				string pattern = string.Join("|", sensitiveWords
					.Select(word => $@"\b{Regex.Escape(word)}\b")
					.Distinct()
				);

				string sanitizedMessage = Regex.Replace(message, pattern, match =>
					new string('*', match.Length), RegexOptions.IgnoreCase | RegexOptions.Compiled);

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
