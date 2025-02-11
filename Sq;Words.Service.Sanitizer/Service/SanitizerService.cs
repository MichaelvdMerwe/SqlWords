﻿
using System.Text.RegularExpressions;

namespace Sq_Words.Service.Sanitizer.Service
{
	public class SanitizerService : ISanitizerService
	{
		public string Sanitize(IEnumerable<string> sensitiveWords, string message)
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				return message;
			}

			foreach (string word in sensitiveWords)
			{
				string pattern = $@"\b{Regex.Escape(word)}\b";
				message = Regex.Replace(message, pattern, "****", RegexOptions.IgnoreCase);
			}

			return message;
		}

		////time complexity O(N*M)
		//private string SanitizeMessageBasic(string message, List<string> forbiddenWords)
		//{
		//	foreach (string word in forbiddenWords)
		//	{
		//		message = message.Replace(word, new string('*', word.Length));
		//	}
		//	return message;
		//}

		////time complexity O(N)
		//private string SanitizeMessageSplit(string message, HashSet<string> forbiddenWords)
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

		//private string SanitizeMessageRegex(string message, List<string> forbiddenWords)
		//{
		//	string pattern = @"\b(" + string.Join("|", forbiddenWords) + @")\b";
		//	return Regex.Replace(message, pattern, m => new string('*', m.Length), RegexOptions.IgnoreCase);
		//}
	}
}
