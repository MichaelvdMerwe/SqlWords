namespace SqlWords.Service.Sanitizer.Service
{
	public interface ISanitizerService
	{
		string Sanitize(IEnumerable<string> sensitiveWords, string message);
	}
}
