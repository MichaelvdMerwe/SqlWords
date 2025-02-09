using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

using Dapper;

namespace SqlWords.Infrastructure.UnitOfWork.Repositories
{
	public class Repository<T>(IDbConnection dbConnection) : IRepository<T> where T : class
	{
		protected readonly IDbConnection _dbConnection = dbConnection;

		public async Task<IEnumerable<T>> GetAllAsync()
		{
			string tableName = GetTableName();
			string sql = $"SELECT * FROM {tableName}";
			return await _dbConnection.QueryAsync<T>(sql);
		}

		public async Task<T?> GetAsync(long id)
		{
			string tableName = GetTableName();
			string sql = $"SELECT * FROM {tableName} WHERE Id = @Id";
			return await _dbConnection.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
		}

		public async Task<long> AddAsync(T entity)
		{
			string sql = GenerateInsertQuery() + "; SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";
			return await _dbConnection.ExecuteScalarAsync<long>(sql, entity);
		}

		public async Task<List<long>> AddRangeAsync(IEnumerable<T> entities)
		{
			string sql = GenerateInsertQuery() + "; SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";
			IEnumerable<long> insertedIds = await _dbConnection.QueryAsync<long>(sql, entities);
			return insertedIds.ToList();
		}

		public async Task<int> UpdateAsync(T entity)
		{
			string sql = GenerateUpdateQuery();
			return await _dbConnection.ExecuteAsync(sql, entity);
		}

		public async Task<int> UpdateRangeAsync(IEnumerable<T> entities)
		{
			string sql = GenerateUpdateQuery();
			return await _dbConnection.ExecuteAsync(sql, entities);
		}

		public async Task<int> DeleteAsync(long id)
		{
			string tableName = GetTableName();
			string sql = $"DELETE FROM {tableName} WHERE Id = @Id";
			return await _dbConnection.ExecuteAsync(sql, new { Id = id });
		}

		public async Task<int> DeleteRangeAsync(IEnumerable<T> entities)
		{
			string tableName = GetTableName();
			string sql = $"DELETE FROM {tableName} WHERE Id IN @Ids";
			List<object?> ids = entities.Select(e => e.GetType().GetProperty("Id")?.GetValue(e)).ToList();
			return await _dbConnection.ExecuteAsync(sql, new { Ids = ids });
		}

		private static string GenerateInsertQuery()
		{
			Type type = typeof(T);
			string tableName = GetTableName();

			IEnumerable<string> properties = type.GetProperties()
				.Where(p => p.Name != "Id") // Skip ID (auto-incremented)
				.Select(p => p.Name);

			string columnNames = string.Join(", ", properties);
			string paramNames = string.Join(", ", properties.Select(p => "@" + p));

			return $"INSERT INTO {tableName} ({columnNames}) VALUES ({paramNames})";
		}

		private static string GenerateUpdateQuery()
		{
			Type type = typeof(T);
			string tableName = GetTableName();

			IEnumerable<string> properties = type.GetProperties()
				.Where(p => p.Name != "Id") // Skip ID in update
				.Select(p => $"{p.Name} = @{p.Name}");

			string setClause = string.Join(", ", properties);
			return $"UPDATE {tableName} SET {setClause} WHERE Id = @Id";
		}

		private static string GetTableName()
		{
			Type type = typeof(T);
			string tableName = type.GetCustomAttributes(typeof(TableAttribute), false)
								   .FirstOrDefault() is TableAttribute tableAttribute
				? tableAttribute.Name
				: type.Name;

			return $"dbo.[{tableName}]"; // Explicit dbo schema
		}
	}
}
