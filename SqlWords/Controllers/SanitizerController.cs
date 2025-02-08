using Microsoft.AspNetCore.Mvc;

using SqlWords.Api.Controllers.Dto.Sanitizer;

using Swashbuckle.AspNetCore.Annotations;

namespace SqlWords.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Produces("application/json")]
	public class SanitizerController(ISanitizerService sanitizerService) : ControllerBase
	{
		private readonly ISanitizerService _sanitizerService = sanitizerService;

		/// <summary>
		/// Sanitizes a sentence by replacing sensitive words with ****.
		/// </summary>
		/// <param name="request">The request containing the sentence to sanitize.</param>
		/// <returns>The sanitized sentence.</returns>
		[HttpPost]
		[SwaggerOperation(Summary = "Sanitize a sentence", Description = "Replaces sensitive words in a sentence with ****.")]
		[SwaggerResponse(200, "Returns the sanitized sentence", typeof(string))]
		public async Task<ActionResult<string>> Sanitize([FromBody] SanitizeRequestDto request)
		{
			var sanitizedText = await _sanitizerService.SanitizeAsync(request.Sentence);
			return Ok(new { sanitizedText });
		}
	}
}
