using FluentValidation;
using FluentValidation.Results;

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
	public class SanitizerController
	(
		IMediator mediator,
		ILogger<SanitizerController> logger,
		IValidator<SanitizeRequestDto> sanitizeRequestValidator
	) : ControllerBase
	{
		private readonly IMediator _mediator = mediator;
		private readonly ILogger<SanitizerController> _logger = logger;
		private readonly IValidator<SanitizeRequestDto> _sanitizeRequestValidator = sanitizeRequestValidator;

		/// <summary>
		/// Sanitizes a sentence by replacing sensitive words with ****.
		/// </summary>
		/// <param name="request">The request containing the sentence to sanitize.</param>
		/// <returns>The sanitized sentence.</returns>
		[HttpPost("sanitize")]
		[SwaggerOperation(Summary = "Sanitize a sentence", Description = "Replaces sensitive words in a sentence with ****.")]
		[SwaggerResponse(200, "Returns the sanitized sentence", typeof(string))]
		[SwaggerResponse(400, "Invalid request")]
		[SwaggerResponse(500, "Internal server error")]
		public async Task<ActionResult<string>> Sanitize([FromBody] SanitizeRequestDto request)
		{
			try
			{
				_logger.LogInformation("Sanitizing message: {Message}", request.Message);

				ValidationResult validationResult = await _sanitizeRequestValidator.ValidateAsync(request);
				if (!validationResult.IsValid)
				{
					return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
				}

				string sanitizedMessage = await _mediator.Send(new SanitizeMessageQuery(request.Message));
				return Ok(new { sanitizedMessage });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while sanitizing message.");
				return StatusCode(500, new { message = "An error occurred while sanitizing the sentence." });
			}
		}
	}
}
