using System.Data;

using Dapper;
using SqlWords.Domain.Entities;
using SqlWords.Infrastructure.UnitOfWork.Repositories;

namespace SqlWords.Infrastructure.UnitOfWork.Repositories.SensitiveWords
{
    public class SensitiveWordRepository(IDbConnection dbConnection)
        : Repository<SensitiveWord>(dbConnection), ISensitiveWordRepository
    {
        public async Task<SensitiveWord?> GetByWordAsync(string word)
        {
            string sql = $"SELECT * FROM {typeof(SensitiveWord).Name} WHERE Word = @Word";
            return await _dbConnection.QuerySingleOrDefaultAsync<SensitiveWord>(sql, new { Word = word });
        }
    }
}