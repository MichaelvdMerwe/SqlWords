namespace Sq_Words.Service.Sanitizer.Service
{
	public interface ISanitizerService
	{
		public string Sanitize(IEnumerable<string> sensitiveWords, string message);
	}
}
