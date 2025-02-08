using MediatR;

using Microsoft.AspNetCore.Mvc;

using SqlWords.Application.Handlers.Commands.CUD.AddSensitiveWord;
using SqlWords.Application.Handlers.Queries.GetAllSensitiveWords;
using SqlWords.Domain.Entities;

using Swashbuckle.AspNetCore.Annotations;

namespace SqlWords.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Produces("application/json")]
	public class AdminController(IMediator mediator) : ControllerBase
	{
		private readonly IMediator _mediator = mediator;

		/// <summary>
		/// Retrieves all sensitive words.
		/// </summary>
		/// <returns>A list of sensitive words.</returns>
		[HttpGet]
		[SwaggerOperation(Summary = "Get all sensitive words", Description = "Retrieves all sensitive words from the database.")]
		[SwaggerResponse(200, "Returns the list of sensitive words", typeof(IEnumerable<SensitiveWord>))]
		public async Task<ActionResult<IEnumerable<SensitiveWord>>> GetAll()
		{
			IEnumerable<SensitiveWord> words = await _mediator.Send(new GetAllSensitiveWordsQuery());
			return Ok(words);
		}

		/// <summary>
		/// Adds a new sensitive word.
		/// </summary>
		/// <param name="command">The sensitive word to add.</param>
		/// <returns>The ID of the created sensitive word.</returns>
		[HttpPost]
		[SwaggerOperation(Summary = "Add a new sensitive word", Description = "Creates a new sensitive word and returns its ID.")]
		[SwaggerResponse(201, "The sensitive word was created successfully", typeof(long))]
		[SwaggerResponse(400, "Invalid input provided")]
		public async Task<ActionResult<long>> Add([FromBody] AddSensitiveWordCommand command)
		{
			long wordId = await _mediator.Send(command);
			return CreatedAtAction(nameof(GetAll), new { id = wordId }, wordId);
		}
	}
}
