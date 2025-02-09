using SqlWords.Domain.Entities;
using SqlWords.Infrastructure.UnitOfWork.Repositories;

namespace SqlWords.Infrastructure.UnitOfWork.Repositories.SensitiveWords
{
    public interface ISensitiveWordRepository : IRepository<SensitiveWord>
    {
        public Task<SensitiveWord?> GetByWordAsync(string word);
    }
}