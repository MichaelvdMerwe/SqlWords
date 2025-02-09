using System.Data;

using Dapper;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

using SqlWords.Domain.Entities;

namespace SqlWords.Infrastructure.UnitOfWork.Repositories.SensitiveWords
{
	public class SensitiveWordRepository(IDbConnection dbConnection, ILogger<SensitiveWordRepository> logger)
		: Repository<SensitiveWord>(dbConnection, logger), ISensitiveWordRepository
	{
		public async Task<SensitiveWord?> GetByWordAsync(string word)
		{
			string tableName = GetTableName();
			string sql = $"SELECT * FROM {tableName} WHERE Word = @Word";

			try
			{
				return await _dbConnection.QuerySingleOrDefaultAsync<SensitiveWord>(sql, new { Word = word });
			}
			catch (SqlException ex)
			{
				_logger.LogError(ex, "SQL error while retrieving sensitive word '{Word}' from {TableName}", word, tableName);
				throw new ApplicationException($"Failed to retrieve word '{word}' from {tableName}.", ex);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error in GetByWordAsync for {TableName}, Word: {Word}", tableName, word);
				throw;
			}
		}

		private static string GetTableName()
		{
			return $"dbo.[{nameof(SensitiveWord)}]";
		}
	}
}
