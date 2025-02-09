using SqlWords.Api.Controllers;
using SqlWords.Api.Controllers.Dto.SensitiveWord;
using SqlWords.Api.Controllers.Dto.SensitiveWords;
using SqlWords.Application.Handlers.Commands.CUD.AddSensitiveWord;
using SqlWords.Application.Handlers.Commands.CUD.DeleteSensitiveWord;
using SqlWords.Application.Handlers.Queries.GetAllSensitiveWords;
using SqlWords.Application.Handlers.Queries.GetSensitiveWordById;
using SqlWords.Domain.Entities;

namespace SqlWords.Api.Tests.Controllers
{
	public class SensitiveWordControllerTests
	{
		private Mock<IMediator> _mockMediator;
		private Mock<ILogger<SensitiveWordController>> _mockLogger;
		private Mock<IValidator<AddSensitiveWordDto>> _mockAddValidator;
		private Mock<IValidator<AddSensitiveWordsDto>> _mockBulkAddValidator;
		private Mock<IValidator<UpdateSensitiveWordDto>> _mockUpdateValidator;
		private Mock<IValidator<UpdateSensitiveWordsDto>> _mockBulkUpdateValidator;
		private Mock<IValidator<DeleteSensitiveWordsDto>> _mockBulkDeleteValidator;
		private SensitiveWordController _controller;

		[SetUp]
		public void Setup()
		{
			_mockMediator = new Mock<IMediator>();
			_mockLogger = new Mock<ILogger<SensitiveWordController>>();
			_mockAddValidator = new Mock<IValidator<AddSensitiveWordDto>>();
			_mockBulkAddValidator = new Mock<IValidator<AddSensitiveWordsDto>>();
			_mockUpdateValidator = new Mock<IValidator<UpdateSensitiveWordDto>>();
			_mockBulkUpdateValidator = new Mock<IValidator<UpdateSensitiveWordsDto>>();
			_mockBulkDeleteValidator = new Mock<IValidator<DeleteSensitiveWordsDto>>();

			_controller = new SensitiveWordController(
				_mockMediator.Object,
				_mockLogger.Object,
				_mockAddValidator.Object,
				_mockBulkAddValidator.Object,
				_mockUpdateValidator.Object,
				_mockBulkUpdateValidator.Object,
				_mockBulkDeleteValidator.Object
			);
		}

		[Test]
		public async Task GetAll_ReturnsListOfWords()
		{
			// Arrange
			List<SensitiveWord> words = [new("test")];
			_ = _mockMediator.Setup(m => m.Send(It.IsAny<GetAllSensitiveWordsQuery>(), It.IsAny<CancellationToken>()))
						 .ReturnsAsync(words);

			// Act
			ActionResult<IEnumerable<SensitiveWord>> result = await _controller.GetAll();

			// Assert
			OkObjectResult? okResult = result.Result as OkObjectResult;
			_ = okResult.Should().NotBeNull();
			_ = okResult.Value.Should().BeEquivalentTo(words);
		}

		[Test]
		public async Task GetAll_ThrowsException_ReturnsInternalServerError()
		{
			// Arrange
			_ = _mockMediator.Setup(m => m.Send(It.IsAny<GetAllSensitiveWordsQuery>(), It.IsAny<CancellationToken>()))
						 .ThrowsAsync(new Exception("DB error"));

			// Act
			ActionResult<IEnumerable<SensitiveWord>> result = await _controller.GetAll();

			// Assert
			ObjectResult? objectResult = result.Result as ObjectResult;
			_ = objectResult.Should().NotBeNull();
			_ = objectResult.StatusCode.Should().Be(500);
		}

		[Test]
		public async Task GetById_ValidId_ReturnsWord()
		{
			// Arrange
			SensitiveWord word = new("test");
			_ = _mockMediator.Setup(m => m.Send(It.IsAny<GetSensitiveWordByIdQuery>(), It.IsAny<CancellationToken>()))
						 .ReturnsAsync(word);

			// Act
			ActionResult<SensitiveWord> result = await _controller.GetById(1);

			// Assert
			OkObjectResult? okResult = result.Result as OkObjectResult;
			_ = okResult.Should().NotBeNull();
			_ = okResult.Value.Should().BeEquivalentTo(word);
		}

		[Test]
		public async Task GetById_InvalidId_ReturnsNotFound()
		{
			// Arrange
			_ = _mockMediator.Setup(m => m.Send(It.IsAny<GetSensitiveWordByIdQuery>(), It.IsAny<CancellationToken>()))
						 .ReturnsAsync((SensitiveWord?)null);

			// Act
			ActionResult<SensitiveWord> result = await _controller.GetById(99);

			// Assert
			NotFoundObjectResult? notFoundResult = result.Result as NotFoundObjectResult;
			_ = notFoundResult.Should().NotBeNull();
		}

		[Test]
		public async Task Add_ValidRequest_ReturnsCreatedResult()
		{
			// Arrange
			AddSensitiveWordDto dto = new() { Word = "test" };
			_ = _mockAddValidator.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
							 .ReturnsAsync(new ValidationResult());
			_ = _mockMediator.Setup(m => m.Send(It.IsAny<AddSensitiveWordCommand>(), It.IsAny<CancellationToken>()))
						 .ReturnsAsync(1);

			// Act
			ActionResult<long> result = await _controller.Add(dto);

			// Assert
			CreatedAtActionResult? createdResult = result.Result as CreatedAtActionResult;
			_ = createdResult.Should().NotBeNull();
			_ = createdResult.Value.Should().Be(1);
		}

		[Test]
		public async Task Add_InvalidRequest_ReturnsBadRequest()
		{
			// Arrange
			AddSensitiveWordDto dto = new() { Word = "" };
			List<ValidationFailure> validationErrors =
			[
				new("Word", "Word cannot be empty.")
			];

			_ = _mockAddValidator.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
							 .ReturnsAsync(new ValidationResult(validationErrors));

			// Act
			ActionResult<long> result = await _controller.Add(dto);

			// Assert
			BadRequestObjectResult? badRequestResult = result.Result as BadRequestObjectResult;
			_ = badRequestResult.Should().NotBeNull();
		}

		[Test]
		public async Task Add_Exception_ReturnsInternalServerError()
		{
			// Arrange
			AddSensitiveWordDto dto = new() { Word = "test" };
			_ = _mockAddValidator.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
							 .ReturnsAsync(new ValidationResult());
			_ = _mockMediator.Setup(m => m.Send(It.IsAny<AddSensitiveWordCommand>(), It.IsAny<CancellationToken>()))
						 .ThrowsAsync(new Exception("DB error"));

			// Act
			ActionResult<long> result = await _controller.Add(dto);

			// Assert
			ObjectResult? objectResult = result.Result as ObjectResult;
			_ = objectResult.Should().NotBeNull();
			_ = objectResult.StatusCode.Should().Be(500);
		}

		[Test]
		public async Task Delete_ExistingWord_ReturnsOk()
		{
			// Arrange
			_ = _mockMediator.Setup(m => m.Send(It.IsAny<DeleteSensitiveWordCommand>(), It.IsAny<CancellationToken>()))
						 .ReturnsAsync(true);

			// Act
			IActionResult result = await _controller.Delete(1);

			// Assert
			OkObjectResult? okResult = result as OkObjectResult;
			_ = okResult.Should().NotBeNull();
		}
	}
}
