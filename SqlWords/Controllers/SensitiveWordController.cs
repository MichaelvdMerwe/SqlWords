using MediatR;

using Microsoft.AspNetCore.Mvc;

using SqlWords.Application.Handlers.Commands.CUD.AddSensitiveWord;
using SqlWords.Application.Handlers.Commands.CUD.AddSensitiveWords;
using SqlWords.Application.Handlers.Commands.CUD.DeleteSensitiveWord;
using SqlWords.Application.Handlers.Commands.CUD.DeleteSensitiveWords;
using SqlWords.Application.Handlers.Commands.CUD.UpdateSensitiveWord;
using SqlWords.Application.Handlers.Commands.CUD.UpdateSensitiveWords;
using SqlWords.Application.Handlers.Queries.GetAllSensitiveWords;
using SqlWords.Application.Handlers.Queries.GetSensitiveWordById;
using SqlWords.Application.Handlers.Queries.GetSensitiveWordByWord;
using SqlWords.Domain.Entities;

using Swashbuckle.AspNetCore.Annotations;




//TODO: Implement DTO objects
//		Implement Validation
//		Implement Logging
//		Implement Exception handling
//		Implement CancelTokens
namespace SqlWords.Api.Controllers
{
	[ApiController]
	[Route("api/admin/[controller]")]
	[Produces("application/json")]
	public class SensitiveWordController(IMediator mediator) : ControllerBase
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
			IEnumerable<SensitiveWord> sensitiveWords = await _mediator.Send(new GetAllSensitiveWordsQuery());
			return Ok(sensitiveWords);
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
			long sensitiveWordId = await _mediator.Send(command);
			return CreatedAtAction(nameof(GetAll), new { id = sensitiveWordId }, sensitiveWordId);
		}

		/// <summary>
		/// Retrieves a specific sensitive word by ID.
		/// </summary>
		/// <param name="id">The ID of the word.</param>
		/// <returns>The sensitive word, if found.</returns>
		[HttpGet("{id}")]
		[SwaggerOperation(Summary = "Get a sensitive word by ID", Description = "Retrieves a single sensitive word by ID.")]
		[SwaggerResponse(200, "Returns the sensitive word", typeof(SensitiveWord))]
		[SwaggerResponse(404, "Sensitive word not found")]
		public async Task<ActionResult<SensitiveWord>> GetById(long id)
		{
			SensitiveWord? sensitiveWord = await _mediator.Send(new GetSensitiveWordByIdQuery(id));
			return sensitiveWord == null ? (ActionResult<SensitiveWord>)NotFound() : (ActionResult<SensitiveWord>)Ok(sensitiveWord);
		}

		/// <summary>
		/// Retrieves a specific sensitive word by ID.
		/// </summary>
		/// <param name="id">The ID of the word.</param>
		/// <returns>The sensitive word, if found.</returns>
		[HttpGet("{word}")]
		[SwaggerOperation(Summary = "Get a sensitive word by ID", Description = "Retrieves a single sensitive word by ID.")]
		[SwaggerResponse(200, "Returns the sensitive word", typeof(SensitiveWord))]
		[SwaggerResponse(404, "Sensitive word not found")]
		public async Task<ActionResult<SensitiveWord>> GetByWord(string word)
		{
			SensitiveWord? sensitiveWord = await _mediator.Send(new GetSensitiveWordByWordQuery(word));
			return word == null ? (ActionResult<SensitiveWord>)NotFound() : (ActionResult<SensitiveWord>)Ok(sensitiveWord);
		}

		/// <summary>
		/// Updates an existing sensitive word.
		/// </summary>
		/// <param name="id">The ID of the word to update.</param>
		/// <param name="command">The new word data.</param>
		/// <returns>Success status.</returns>
		[HttpPut("{id}")]
		[SwaggerOperation(Summary = "Update a sensitive word", Description = "Updates an existing sensitive word by ID.")]
		[SwaggerResponse(200, "Word updated successfully")]
		[SwaggerResponse(404, "Sensitive word not found")]
		public async Task<IActionResult> Update(long id, [FromBody] UpdateSensitiveWordCommand command)
		{
			if (id != command.Id)
			{
				return BadRequest("ID mismatch");
			}

			bool success = await _mediator.Send(command);
			return !success ? NotFound() : Ok();
		}

		/// <summary>
		/// Deletes a sensitive word by ID.
		/// </summary>
		/// <param name="id">The ID of the word to delete.</param>
		/// <returns>Success status.</returns>
		[HttpDelete("{id}")]
		[SwaggerOperation(Summary = "Delete a sensitive word", Description = "Deletes an existing sensitive word by ID.")]
		[SwaggerResponse(200, "Word deleted successfully")]
		[SwaggerResponse(404, "Sensitive word not found")]
		public async Task<IActionResult> Delete(long id)
		{
			bool success = await _mediator.Send(new DeleteSensitiveWordCommand(id));
			return !success ? NotFound() : Ok();
		}

		/// <summary>
		/// Adds multiple sensitive words.
		/// </summary>
		[HttpPost("bulk")]
		[SwaggerOperation(Summary = "Add multiple sensitive words", Description = "Creates multiple sensitive words at once.")]
		[SwaggerResponse(201, "The sensitive words were created successfully", typeof(List<long>))]
		public async Task<ActionResult<List<long>>> AddRange([FromBody] AddSensitiveWordsCommand command)
		{
			List<long> wordIds = await _mediator.Send(command);
			return Created("", wordIds);
		}

		/// <summary>
		/// Updates multiple sensitive words.
		/// </summary>
		[HttpPut("bulk")]
		[SwaggerOperation(Summary = "Update multiple sensitive words", Description = "Updates multiple sensitive words at once.")]
		[SwaggerResponse(200, "Words updated successfully")]
		[SwaggerResponse(404, "One or more sensitive words not found")]
		public async Task<IActionResult> UpdateRange([FromBody] UpdateSensitiveWordsCommand command)
		{
			bool success = await _mediator.Send(command);
			return !success ? NotFound() : Ok();
		}

		/// <summary>
		/// Deletes multiple sensitive words.
		/// </summary>
		[HttpDelete("bulk")]
		[SwaggerOperation(Summary = "Delete multiple sensitive words", Description = "Deletes multiple sensitive words at once.")]
		[SwaggerResponse(200, "Words deleted successfully")]
		[SwaggerResponse(404, "One or more sensitive words not found")]
		public async Task<IActionResult> DeleteRange([FromBody] DeleteSensitiveWordsCommand command)
		{
			bool success = await _mediator.Send(command);
			return !success ? NotFound() : Ok();
		}
	}
}
