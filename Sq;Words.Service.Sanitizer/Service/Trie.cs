using System.Text;

namespace Sq_Words.Service.Sanitizer.Service
{
	internal class Trie
	{
		private readonly TrieNode _root = new();

		public void Insert(string word)
		{
			TrieNode node = _root;
			foreach (char ch in word)
			{
				if (!node.Children.TryGetValue(ch, out TrieNode? value))
				{
					value = new TrieNode();
					node.Children[ch] = value;
				}

				node = value;
			}
			node.IsEndOfWord = true;
		}

		public string Sanitize(string message)
		{
			StringBuilder result = new(message);
			for (int i = 0; i < message.Length; i++)
			{
				TrieNode node = _root;
				int j = i;

				while (j < message.Length && node.Children.ContainsKey(message[j]))
				{
					node = node.Children[message[j]];
					if (node.IsEndOfWord)
					{
						for (int k = i; k <= j; k++)
						{
							result[k] = '*';
						}

						break;
					}
					j++;
				}
			}
			return result.ToString();
		}
	}
}
