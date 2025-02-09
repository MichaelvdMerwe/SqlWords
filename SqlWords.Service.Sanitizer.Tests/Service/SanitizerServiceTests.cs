using SqlWords.Service.Sanitizer.Service;

namespace SqlWords.Service.Sanitizer.Tests.Service
{
	public class SanitizerServiceTests
	{
		private Mock<ILogger<SanitizerService>> _mockLogger;
		private SanitizerService _sanitizerService;

		[SetUp]
		public void Setup()
		{
			_mockLogger = new Mock<ILogger<SanitizerService>>();
			_sanitizerService = new SanitizerService(_mockLogger.Object);
		}

		[Test]
		public void Sanitize_SingleSensitiveWord_ShouldMaskWord()
		{
			// Arrange
			List<string> words = ["SELECT"];
			string input = "I want to SELECT something";

			// Act
			string result = _sanitizerService.Sanitize(words, input);

			// Assert
			_ = result.Should().Be("I want to ****** something");
		}

		[Test]
		public void Sanitize_MultipleSensitiveWords_ShouldMaskAll()
		{
			// Arrange
			List<string> words = ["SELECT", "FROM", "WHERE"];
			string input = "SELECT * FROM Users WHERE Name='John'";

			// Act
			string result = _sanitizerService.Sanitize(words, input);

			// Assert
			_ = result.Should().Be("****** * **** Users ***** Name='John'");
		}

		[Test]
		public void Sanitize_CaseInsensitiveMatch_ShouldMaskAll()
		{
			// Arrange
			List<string> words = ["select", "from", "where"];
			string input = "SeLecT * fRoM Users WherE Name='John'";

			// Act
			string result = _sanitizerService.Sanitize(words, input);

			// Assert
			_ = result.Should().Be("****** * **** Users ***** Name='John'");
		}

		[Test]
		public void Sanitize_QuotedWords_ShouldMaskProperly()
		{
			// Arrange
			List<string> words = ["ALTER", "EGO"];
			string input = "User has \"AlTer Ego\" abilities.";

			// Act
			string result = _sanitizerService.Sanitize(words, input);

			// Assert
			_ = result.Should().Be("User has \"***** ***\" abilities.");
		}

		[Test]
		public void Sanitize_WordsWithPunctuation_ShouldMaskCorrectly()
		{
			// Arrange
			List<string> words = ["DROP", "TABLE"];
			string input = "DROP; TABLE users;";

			// Act
			string result = _sanitizerService.Sanitize(words, input);

			// Assert
			_ = result.Should().Be("****; ***** users;");
		}

		[Test]
		public void Sanitize_EmptyMessage_ShouldReturnOriginal()
		{
			// Arrange
			List<string> words = ["SELECT"];

			// Act
			string result1 = _sanitizerService.Sanitize(words, "");

			// Assert
			_ = result1.Should().Be("");
		}

		[Test]
		public void Sanitize_NoSensitiveWordsInSentence_ShouldReturnOriginal()
		{
			// Arrange
			List<string> words = ["ALTER", "DELETE"];
			string input = "This is a normal sentence.";

			// Act
			string result = _sanitizerService.Sanitize(words, input);

			// Assert
			_ = result.Should().Be(input);
		}

		[Test]
		public void Sanitize_ComplexSQLQuery_ShouldMaskAllKeywords()
		{
			// Arrange
			List<string> words = ["SELECT", "FROM", "WHERE", "DELETE", "UPDATE"];
			string input = "SELECT * FROM users WHERE id = 5; UPDATE users SET name='John' WHERE id = 5; DELETE FROM users WHERE id = 5;";

			// Act
			string result = _sanitizerService.Sanitize(words, input);

			// Assert
			_ = result.Should().Be("****** * **** users ***** id = 5; ****** users SET name='John' ***** id = 5; ****** **** users ***** id = 5;");
		}
	}
}
