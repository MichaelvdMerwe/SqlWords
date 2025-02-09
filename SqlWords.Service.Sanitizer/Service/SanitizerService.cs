using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

namespace SqlWords.Service.Sanitizer.Service
{
	public class SanitizerService(ILogger<SanitizerService> logger) : ISanitizerService
	{
		private readonly ILogger<SanitizerService> _logger = logger;

		//not entirely happy with this sanitizer. Will need to look at performance, there is also an edge case that i just cant seem to crack with the rejex
		public string Sanitize(IEnumerable<string> sensitiveWords, string message)
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				_logger.LogWarning("Sanitize method received an empty or null message.");
				return message;
			}

			try
			{
				_logger.LogInformation("Sanitizing message.");

				string sanitizedMessage = message;

				foreach (string word in sensitiveWords)
				{
					string pattern = $@"\b{Regex.Escape(word)}\b";

					sanitizedMessage = Regex.Replace(
						sanitizedMessage,
						pattern,
						match => new string('*', match.Length),
						RegexOptions.IgnoreCase | RegexOptions.CultureInvariant
					);
				}

				_logger.LogInformation("Message sanitized successfully.");
				return sanitizedMessage;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while sanitizing message.");
				return message;
			}
		}
	}
}

#region ALGORITHM IDEAS
////time complexity O(N*M)
//public static string SanitizeMessageBasic(string message, List<string> forbiddenWords)
//{
//	foreach (string word in forbiddenWords)
//	{
//		message = message.Replace(word, new string('*', word.Length));
//	}
//	return message;
//}


////time complexity O(N)
//public static string SanitizeMessageSplit(string message, HashSet<string> forbiddenWords)
//{
//	string[] words = message.Split(' ');
//	for (int i = 0; i < words.Length; i++)
//	{
//		string strippedWord = words[i].TrimEnd('.', ',', '!', '?'); // Handle punctuation
//		if (forbiddenWords.Contains(strippedWord.ToLower()))
//		{
//			words[i] = new string('*', strippedWord.Length);
//		}
//	}
//	return string.Join(" ", words);
//}


//// I need to test and figure this one out.
////https://code-maze.com/csharp-using-trie-class-for-efficient-text-pattern-searching/
////https://en.wikipedia.org/wiki/Trie
//public class TrieNode
//{
//	public Dictionary<char, TrieNode> Children = [];
//	public bool IsEndOfWord;
//}

//public class Trie
//{
//	private readonly TrieNode _root = new();

//	public void Insert(string word)
//	{
//		TrieNode node = _root;
//		foreach (char ch in word)
//		{
//			if (!node.Children.ContainsKey(ch))
//			{
//				node.Children[ch] = new TrieNode();
//			}

//			node = node.Children[ch];
//		}
//		node.IsEndOfWord = true;
//	}

//	public string Sanitize(string message)
//	{
//		StringBuilder result = new(message);
//		for (int i = 0; i < message.Length; i++)
//		{
//			TrieNode node = _root;
//			int j = i;

//			while (j < message.Length && node.Children.ContainsKey(message[j]))
//			{
//				node = node.Children[message[j]];
//				if (node.IsEndOfWord)
//				{
//					for (int k = i; k <= j; k++)
//					{
//						result[k] = '*';
//					}

//					break;
//				}
//				j++;
//			}
//		}
//		return result.ToString();
//	}
//}
#endregion
