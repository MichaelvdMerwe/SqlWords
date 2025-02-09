using FluentValidation;
using FluentValidation.Results;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using SqlWords.Api.Controllers.Dto.SensitiveWord;
using SqlWords.Api.Controllers.Dto.SensitiveWords;
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

namespace SqlWords.Api.Controllers
{
	[ApiController]
	[Route("api/admin/[controller]")]
	[Produces("application/json")]
	public class SensitiveWordController(
		IMediator mediator,
		ILogger<SensitiveWordController> logger,
		IValidator<AddSensitiveWordDto> addValidator,
		IValidator<AddSensitiveWordsDto> bulkAddValidator,
		IValidator<UpdateSensitiveWordDto> updateValidator,
		IValidator<UpdateSensitiveWordsDto> bulkUpdateValidator,
		IValidator<DeleteSensitiveWordsDto> bulkDeleteValidator) : ControllerBase
	{
		private readonly IMediator _mediator = mediator;
		private readonly ILogger<SensitiveWordController> _logger = logger;
		private readonly IValidator<AddSensitiveWordDto> _addValidator = addValidator;
		private readonly IValidator<AddSensitiveWordsDto> _bulkAddValidator = bulkAddValidator;
		private readonly IValidator<UpdateSensitiveWordDto> _updateValidator = updateValidator;
		private readonly IValidator<UpdateSensitiveWordsDto> _bulkUpdateValidator = bulkUpdateValidator;
		private readonly IValidator<DeleteSensitiveWordsDto> _bulkDeleteValidator = bulkDeleteValidator;

		/// <summary> Retrieves all sensitive words. </summary>
		[HttpGet]
		[SwaggerOperation(Summary = "Get all sensitive words", Description = "Retrieves all sensitive words from the database.")]
		[SwaggerResponse(200, "Returns the list of sensitive words", typeof(IEnumerable<SensitiveWord>))]
		[SwaggerResponse(500, "Internal server error")]
		public async Task<ActionResult<IEnumerable<SensitiveWord>>> GetAll()
		{
			try
			{
				_logger.LogInformation("Fetching all sensitive words.");
				IEnumerable<SensitiveWord> words = await _mediator.Send(new GetAllSensitiveWordsQuery());
				return Ok(words);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving sensitive words.");
				return StatusCode(500, new { message = "An error occurred while fetching the words." });
			}
		}

		[HttpGet("id/{id:long}")]
		[SwaggerOperation(Summary = "Get a sensitive word by ID", Description = "Retrieves a specific sensitive word from the database by its ID.")]
		[SwaggerResponse(200, "Returns the sensitive word", typeof(SensitiveWord))]
		[SwaggerResponse(404, "Word not found")]
		[SwaggerResponse(500, "Internal server error")]
		public async Task<ActionResult<SensitiveWord>> GetById([FromRoute] long id)
		{
			try
			{
				_logger.LogInformation("Fetching sensitive word with ID: {Id}", id);

				SensitiveWord? word = await _mediator.Send(new GetSensitiveWordByIdQuery(id));

				if (word is null)
				{
					_logger.LogWarning("Sensitive word with ID {Id} not found", id);
					return NotFound(new { message = "Word not found." });
				}

				_logger.LogInformation("Successfully retrieved sensitive word with ID: {Id}", id);
				return Ok(word);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while fetching sensitive word with ID: {Id}", id);
				return StatusCode(500, new { message = "An unexpected error occurred." });
			}
		}

		/// <summary> Retrieves a specific sensitive word by its value. </summary>
		[HttpGet("word/{word}")]
		public async Task<ActionResult<SensitiveWord>> GetByWord([FromRoute] string word)
		{
			SensitiveWord? sensitiveWord = await _mediator.Send(new GetSensitiveWordByWordQuery(word));
			return sensitiveWord is null ? NotFound(new { message = "Word not found." }) : Ok(sensitiveWord);
		}

		/// <summary> Adds a new sensitive word. </summary>
		[HttpPost]
		[SwaggerOperation(Summary = "Add a new sensitive word", Description = "Creates a new sensitive word and returns its ID.")]
		[SwaggerResponse(201, "The sensitive word was created successfully", typeof(long))]
		[SwaggerResponse(400, "Invalid input provided")]
		[SwaggerResponse(500, "Internal server error")]
		public async Task<ActionResult<long>> Add([FromBody] AddSensitiveWordDto dto)
		{
			try
			{
				ValidationResult validationResult = await _addValidator.ValidateAsync(dto);
				if (!validationResult.IsValid)
				{
					_logger.LogWarning("Validation failed for AddSensitiveWordDto: {Errors}", validationResult.Errors);
					return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
				}

				long wordId = await _mediator.Send(new AddSensitiveWordCommand(dto.Word));
				_logger.LogInformation("Successfully added a new sensitive word with ID: {WordId}", wordId);

				return CreatedAtAction(nameof(Add), new { id = wordId }, wordId);
			}
			catch (ArgumentException ex)
			{
				_logger.LogWarning(ex, "Invalid input while adding sensitive word");
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error occurred while adding sensitive word");
				return StatusCode(500, new { message = "An unexpected error occurred." });
			}
		}

		[HttpPost("bulk")]
		[SwaggerOperation(Summary = "Add multiple sensitive words", Description = "Adds multiple sensitive words at once.")]
		[SwaggerResponse(201, "Words added successfully", typeof(int))]
		[SwaggerResponse(400, "Validation failed")]
		[SwaggerResponse(500, "Internal server error")]
		public async Task<ActionResult<int>> AddRange([FromBody] AddSensitiveWordsDto dto)
		{
			try
			{
				_logger.LogInformation("Received request to add {Count} sensitive words.", dto.Words.Count);

				ValidationResult validationResult = await _bulkAddValidator.ValidateAsync(dto);
				if (!validationResult.IsValid)
				{
					_logger.LogWarning("Validation failed for adding multiple sensitive words.");
					return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
				}

				int affectedRows = await _mediator.Send(new AddSensitiveWordsCommand(dto.Words));

				_logger.LogInformation("Successfully added {Count} sensitive words.", affectedRows);
				return CreatedAtAction(nameof(AddRange), new { affectedRows }, affectedRows);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while adding multiple sensitive words.");
				return StatusCode(500, new { message = "An unexpected error occurred while adding words." });
			}
		}

		/// <summary> Updates an existing sensitive word. </summary>
		[HttpPut("{id:long}")]
		[SwaggerOperation(Summary = "Update a sensitive word", Description = "Updates an existing sensitive word by ID.")]
		[SwaggerResponse(200, "Word updated successfully")]
		[SwaggerResponse(400, "Validation failed")]
		[SwaggerResponse(404, "Word not found")]
		[SwaggerResponse(500, "Internal server error")]
		public async Task<IActionResult> Update([FromRoute] long id, [FromBody] UpdateSensitiveWordDto dto)
		{
			try
			{
				_logger.LogInformation("Received request to update word with ID {Id}.", id);

				if (id != dto.Id)
				{
					_logger.LogWarning("ID mismatch: Route ID {RouteId} does not match Body ID {BodyId}.", id, dto.Id);
					return BadRequest(new { message = "ID mismatch between route and body." });
				}

				ValidationResult validationResult = await _updateValidator.ValidateAsync(dto);
				if (!validationResult.IsValid)
				{
					_logger.LogWarning("Validation failed for updating word with ID {Id}.", id);
					return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
				}

				bool success = await _mediator.Send(new UpdateSensitiveWordCommand(dto.Id, dto.Word));

				if (!success)
				{
					_logger.LogWarning("No word found with ID {Id}.", id);
					return NotFound(new { message = "Word not found." });
				}

				_logger.LogInformation("Successfully updated word with ID {Id}.", id);
				return Ok(new { message = "Word updated successfully." });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while updating word with ID {Id}.", id);
				return StatusCode(500, new { message = "An unexpected error occurred while updating the word." });
			}
		}

		/// <summary> Updates multiple sensitive words. </summary>
		/// currently not working, need to figure out the dapper implementations for this kind of feature
		[Obsolete("This API endpoint is not currently supported and will be implemented in a future release.")]
		[ApiExplorerSettings(IgnoreApi = true)]
		[HttpPut("bulk")]
		[SwaggerOperation(Summary = "Update multiple sensitive words", Description = "Updates multiple sensitive words at once.")]
		[SwaggerResponse(200, "Words updated successfully")]
		[SwaggerResponse(400, "Validation failed")]
		[SwaggerResponse(404, "One or more sensitive words not found")]
		[SwaggerResponse(500, "Internal server error")]
		public async Task<IActionResult> UpdateRange([FromBody] UpdateSensitiveWordsDto dto)
		{
			try
			{
				_logger.LogInformation("Received request to update multiple words.");

				ValidationResult validationResult = await _bulkUpdateValidator.ValidateAsync(dto);
				if (!validationResult.IsValid)
				{
					_logger.LogWarning("Validation failed for bulk update.");
					return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
				}

				bool success = await _mediator.Send(new UpdateSensitiveWordsCommand(dto.Words));

				if (!success)
				{
					_logger.LogWarning("One or more words were not found during bulk update.");
					return NotFound(new { message = "One or more words were not found." });
				}

				_logger.LogInformation("Successfully updated multiple words.");
				return Ok(new { message = "Words updated successfully." });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while updating multiple words.");
				return StatusCode(500, new { message = "An unexpected error occurred while updating words." });
			}
		}

		/// <summary> Deletes a sensitive word by ID. </summary>
		[HttpDelete("{id:long}")]
		[SwaggerOperation(Summary = "Delete a sensitive word", Description = "Deletes an existing sensitive word by ID.")]
		[SwaggerResponse(200, "Word deleted successfully")]
		[SwaggerResponse(404, "Sensitive word not found")]
		[SwaggerResponse(500, "Internal server error")]
		public async Task<IActionResult> Delete([FromRoute] long id)
		{
			try
			{
				_logger.LogInformation("Received request to delete word with ID: {WordId}", id);

				// Execute delete command
				bool success = await _mediator.Send(new DeleteSensitiveWordCommand(id));

				if (!success)
				{
					_logger.LogWarning("Word with ID {WordId} not found.", id);
					return NotFound(new { message = "Word not found." });
				}

				_logger.LogInformation("Successfully deleted word with ID: {WordId}", id);
				return Ok(new { message = "Word deleted successfully." });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while deleting word with ID: {WordId}", id);
				return StatusCode(500, new { message = "An unexpected error occurred while deleting word." });
			}
		}

		/// <summary> Deletes multiple sensitive words. </summary>
		/// currently not working, need to figure out the dapper implementations for this kind of feature
		[Obsolete("This API endpoint is not currently supported and will be implemented in a future release.")]
		[ApiExplorerSettings(IgnoreApi = true)]
		[HttpDelete("bulk")]
		[SwaggerOperation(Summary = "Delete multiple sensitive words", Description = "Deletes multiple sensitive words at once.")]
		[SwaggerResponse(200, "Words deleted successfully")]
		[SwaggerResponse(400, "Validation failed")]
		[SwaggerResponse(404, "One or more words were not found")]
		[SwaggerResponse(500, "Internal server error")]
		public async Task<IActionResult> DeleteRange([FromBody] DeleteSensitiveWordsDto dto)
		{
			try
			{
				_logger.LogInformation("Received request to bulk delete words: {WordIds}", dto.Ids);

				// Validate request
				ValidationResult validationResult = await _bulkDeleteValidator.ValidateAsync(dto);
				if (!validationResult.IsValid)
				{
					_logger.LogWarning("Validation failed for bulk delete request.");
					return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
				}

				// Execute delete command
				bool success = await _mediator.Send(new DeleteSensitiveWordsCommand(dto.Ids));

				if (!success)
				{
					_logger.LogWarning("One or more words not found for deletion: {WordIds}", dto.Ids);
					return NotFound(new { message = "One or more words were not found." });
				}

				_logger.LogInformation("Successfully deleted words: {WordIds}", dto.Ids);
				return Ok(new { message = "Words deleted successfully." });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while deleting words: {WordIds}", dto.Ids);
				return StatusCode(500, new { message = "An unexpected error occurred while deleting words." });
			}
		}
	}
}
