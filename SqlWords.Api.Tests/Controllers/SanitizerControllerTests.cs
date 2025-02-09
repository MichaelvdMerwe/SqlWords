using SqlWords.Api.Controllers;
using SqlWords.Api.Controllers.Dto.Sanitizer;
using SqlWords.Application.Handlers.Queries.SanitizeMessage;

namespace SqlWords.Api.Tests.Controllers
{
	[TestFixture]
	public class SanitizerControllerTests
	{
		private Mock<IMediator> _mockMediator;
		private Mock<ILogger<SanitizerController>> _mockLogger;
		private Mock<IValidator<SanitizeRequestDto>> _mockValidator;

		private SanitizerController _controller;

		[SetUp]
		public void Setup()
		{
			_mockMediator = new Mock<IMediator>();
			_mockLogger = new Mock<ILogger<SanitizerController>>();
			_mockValidator = new Mock<IValidator<SanitizeRequestDto>>();

			_controller = new SanitizerController(_mockMediator.Object, _mockLogger.Object, _mockValidator.Object);
		}

		[Test]
		public async Task Sanitize_ValidRequest_ReturnsSanitizedMessage()
		{
			// Arrange
			SanitizeRequestDto request = new() { Message = "This is a SELECT statement." };

			_ = _mockValidator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ValidationResult()); // No validation errors

			_ = _mockMediator.Setup(m => m.Send(It.IsAny<SanitizeMessageQuery>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync("This is a **** statement.");

			// Act
			ActionResult<string> result = await _controller.Sanitize(request);

			OkObjectResult? okResult = result.Result as OkObjectResult;

			// Assert
			Assert.That(okResult, Is.Not.Null);
			Assert.That(okResult.Value, Is.Not.Null);
			Assert.That(okResult.Value, Has.Property("sanitizedMessage").EqualTo("This is a **** statement."));
		}

		[Test]
		public async Task Sanitize_InvalidRequest_ReturnsBadRequest()
		{
			// Arrange
			SanitizeRequestDto request = new() { Message = "" };

			List<ValidationFailure> validationErrors =
			[
				new("Message", "Message cannot be empty.")
			];

			_ = _mockValidator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ValidationResult(validationErrors));

			// Act
			ActionResult<string> result = await _controller.Sanitize(request);

			BadRequestObjectResult? badRequestResult = result.Result as BadRequestObjectResult;

			// Assert
			Assert.That(badRequestResult, Is.Not.Null);
			Assert.That(badRequestResult.Value, Is.Not.Null);
			IEnumerable<string>? errors = badRequestResult.Value as IEnumerable<string>;
			Assert.That(errors, Has.Exactly(1).EqualTo("Message cannot be empty."));
		}

		[Test]
		public async Task Sanitize_MediatorThrowsException_ReturnsInternalServerError()
		{
			// Arrange
			SanitizeRequestDto request = new() { Message = "SELECT * FROM users;" };

			_ = _mockValidator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ValidationResult());

			_ = _mockMediator.Setup(m => m.Send(It.IsAny<SanitizeMessageQuery>(), It.IsAny<CancellationToken>()))
				.ThrowsAsync(new Exception("Database error"));

			// Act
			ActionResult<string> result = await _controller.Sanitize(request);

			ObjectResult? objectResult = result.Result as ObjectResult;

			// Assert
			Assert.That(objectResult, Is.Not.Null);
			Assert.Multiple(() =>
			{
				Assert.That(objectResult.StatusCode, Is.EqualTo(500));
				Assert.That(objectResult.Value, Has.Property("message").EqualTo("An error occurred while sanitizing the sentence."));
			});
		}
	}
}
