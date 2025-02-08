namespace Sq_Words.Service.Sanitizer.Service
{
	internal class TrieNode
	{
		public Dictionary<char, TrieNode> Children = [];
		public bool IsEndOfWord;
	}
}
