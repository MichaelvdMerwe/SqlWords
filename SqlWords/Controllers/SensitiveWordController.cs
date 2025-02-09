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

		/// <summary> Retrieves a specific sensitive word by ID. </summary>
		[HttpGet("id/{id:long}")]
		public async Task<ActionResult<SensitiveWord>> GetById([FromRoute] long id)
		{
			SensitiveWord? word = await _mediator.Send(new GetSensitiveWordByIdQuery(id));
			return word is null ? NotFound(new { message = "Word not found." }) : Ok(word);
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
		public async Task<ActionResult<long>> Add([FromBody] AddSensitiveWordDto dto)
		{
			ValidationResult validationResult = await _addValidator.ValidateAsync(dto);
			if (!validationResult.IsValid)
			{
				return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
			}

			long wordId = await _mediator.Send(new AddSensitiveWordCommand(dto.Word));
			return CreatedAtAction(nameof(Add), new { id = wordId }, wordId);
		}

		/// <summary> Adds multiple sensitive words. </summary>
		[HttpPost("bulk")]
		public async Task<ActionResult<List<long>>> AddRange([FromBody] AddSensitiveWordsDto dto)
		{
			ValidationResult validationResult = await _bulkAddValidator.ValidateAsync(dto);
			if (!validationResult.IsValid)
			{
				return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
			}

			List<long> wordIds = await _mediator.Send(new AddSensitiveWordsCommand(dto.Words));
			return Created(nameof(AddRange), wordIds);
		}

		/// <summary> Updates an existing sensitive word. </summary>
		[HttpPut("{id:long}")]
		public async Task<IActionResult> Update([FromRoute] long id, [FromBody] UpdateSensitiveWordDto dto)
		{
			if (id != dto.Id)
			{
				return BadRequest(new { message = "ID mismatch between route and body." });
			}

			ValidationResult validationResult = await _updateValidator.ValidateAsync(dto);
			if (!validationResult.IsValid)
			{
				return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
			}

			bool success = await _mediator.Send(new UpdateSensitiveWordCommand(dto.Id, dto.Word));
			return success ? Ok() : NotFound(new { message = "Word not found." });
		}

		/// <summary> Updates multiple sensitive words. </summary>
		[HttpPut("bulk")]
		public async Task<IActionResult> UpdateRange([FromBody] UpdateSensitiveWordsDto dto)
		{
			ValidationResult validationResult = await _bulkUpdateValidator.ValidateAsync(dto);
			if (!validationResult.IsValid)
			{
				return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
			}

			bool success = await _mediator.Send(new UpdateSensitiveWordsCommand(dto.Words));
			return success ? Ok() : NotFound(new { message = "One or more words were not found." });
		}

		/// <summary> Deletes a sensitive word by ID. </summary>
		[HttpDelete("{id:long}")]
		public async Task<IActionResult> Delete([FromRoute] long id)
		{
			bool success = await _mediator.Send(new DeleteSensitiveWordCommand(id));
			return success ? Ok() : NotFound(new { message = "Word not found." });
		}

		/// <summary> Deletes multiple sensitive words. </summary>
		[HttpDelete("bulk")]
		public async Task<IActionResult> DeleteRange([FromBody] DeleteSensitiveWordsDto dto)
		{
			ValidationResult validationResult = await _bulkDeleteValidator.ValidateAsync(dto);
			if (!validationResult.IsValid)
			{
				return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
			}

			bool success = await _mediator.Send(new DeleteSensitiveWordsCommand(dto.Ids));
			return success ? Ok() : NotFound(new { message = "One or more words were not found." });
		}
	}
}
