using MediatR;

using Microsoft.AspNetCore.Mvc;

using SqlWords.Api.Controllers.Dto.Sanitizer;
using SqlWords.Application.Handlers.Queries.SanitizeMessage;

using Swashbuckle.AspNetCore.Annotations;

namespace SqlWords.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Produces("application/json")]
	public class SanitizerController(IMediator mediator) : ControllerBase
	{
		private readonly IMediator _mediator = mediator;

		/// <summary>
		/// Sanitizes a sentence by replacing sensitive words with ****.
		/// </summary>
		/// <param name="request">The request containing the sentence to sanitize.</param>
		/// <returns>The sanitized sentence.</returns>
		[HttpPost("sanitize")]
		[SwaggerOperation(Summary = "Sanitize a sentence", Description = "Replaces sensitive words in a sentence with ****.")]
		[SwaggerResponse(200, "Returns the sanitized sentence", typeof(string))]
		public async Task<ActionResult<string>> Sanitize([FromBody] SanitizeRequestDto request)
		{
			string sanitizedMessage = await _mediator.Send(new SanitizeMessageQuery(request.Message));
			return Ok(new { sanitizedMessage });
		}
	}
}
