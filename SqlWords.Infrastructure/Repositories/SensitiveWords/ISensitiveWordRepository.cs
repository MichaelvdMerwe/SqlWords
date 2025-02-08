using SqlWords.Domain.Entities;

namespace SqlWords.Infrastructure.Repositories.SensitiveWords
{
	public interface ISensitiveWordRepository : IRepository<SensitiveWord>
	{
		public Task<SensitiveWord?> GetByWordAsync(string word);
	}
}