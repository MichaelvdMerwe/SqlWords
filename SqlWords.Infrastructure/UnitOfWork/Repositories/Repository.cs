using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

using Dapper;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace SqlWords.Infrastructure.UnitOfWork.Repositories
{
	public class Repository<T>(IDbConnection dbConnection, ILogger<Repository<T>> logger) : IRepository<T> where T : class
	{
		protected readonly IDbConnection _dbConnection = dbConnection;
		protected readonly ILogger<Repository<T>> _logger = logger;

		public async Task<IEnumerable<T>> GetAllAsync()
		{
			string tableName = GetTableName();
			string sql = $"SELECT * FROM {tableName}";

			try
			{
				return await _dbConnection.QueryAsync<T>(sql);
			}
			catch (SqlException ex)
			{
				_logger.LogError(ex, "SQL error while fetching all records from {TableName}", tableName);
				throw new ApplicationException($"Database query failed for {tableName}.", ex);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error in GetAllAsync for {TableName}", tableName);
				throw;
			}
		}

		public async Task<T?> GetAsync(long id)
		{
			string tableName = GetTableName();
			string sql = $"SELECT * FROM {tableName} WHERE Id = @Id";

			try
			{
				return await _dbConnection.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
			}
			catch (SqlException ex)
			{
				_logger.LogError(ex, "SQL error while fetching record {Id} from {TableName}", id, tableName);
				throw new ApplicationException($"Database query failed for {tableName}.", ex);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error in GetAsync for {TableName} with Id {Id}", tableName, id);
				throw;
			}
		}

		public async Task<long> AddAsync(T entity)
		{
			string sql = Repository<T>.GenerateInsertQuery() + "; SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

			try
			{
				return await _dbConnection.ExecuteScalarAsync<long>(sql, entity);
			}
			catch (SqlException ex)
			{
				_logger.LogError(ex, "SQL error while inserting into {TableName}", GetTableName());
				throw new ApplicationException("Database insert operation failed.", ex);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error in AddAsync");
				throw;
			}
		}

		public async Task<List<long>> AddRangeAsync(IEnumerable<T> entities)
		{
			string sql = Repository<T>.GenerateInsertQuery() + "; SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

			try
			{
				IEnumerable<long> insertedIds = await _dbConnection.QueryAsync<long>(sql, entities);
				return insertedIds.ToList();
			}
			catch (SqlException ex)
			{
				_logger.LogError(ex, "SQL error while inserting multiple records into {TableName}", GetTableName());
				throw new ApplicationException("Database bulk insert operation failed.", ex);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error in AddRangeAsync");
				throw;
			}
		}

		public async Task<int> UpdateAsync(T entity)
		{
			string sql = Repository<T>.GenerateUpdateQuery();

			try
			{
				return await _dbConnection.ExecuteAsync(sql, entity);
			}
			catch (SqlException ex)
			{
				_logger.LogError(ex, "SQL error while updating {TableName}", GetTableName());
				throw new ApplicationException("Database update operation failed.", ex);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error in UpdateAsync");
				throw;
			}
		}

		public async Task<int> UpdateRangeAsync(IEnumerable<T> entities)
		{
			string sql = Repository<T>.GenerateUpdateQuery();

			try
			{
				return await _dbConnection.ExecuteAsync(sql, entities);
			}
			catch (SqlException ex)
			{
				_logger.LogError(ex, "SQL error while updating multiple records in {TableName}", GetTableName());
				throw new ApplicationException("Database bulk update operation failed.", ex);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error in UpdateRangeAsync");
				throw;
			}
		}

		public async Task<int> DeleteAsync(long id)
		{
			string tableName = GetTableName();
			string sql = $"DELETE FROM {tableName} WHERE Id = @Id";

			try
			{
				return await _dbConnection.ExecuteAsync(sql, new { Id = id });
			}
			catch (SqlException ex)
			{
				_logger.LogError(ex, "SQL error while deleting record {Id} from {TableName}", id, tableName);
				throw new ApplicationException("Database delete operation failed.", ex);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error in DeleteAsync for {TableName}", tableName);
				throw;
			}
		}

		public async Task<int> DeleteRangeAsync(IEnumerable<T> entities)
		{
			string tableName = GetTableName();
			string sql = $"DELETE FROM {tableName} WHERE Id IN @Ids";

			try
			{
				List<object?> ids = entities.Select(e => e.GetType().GetProperty("Id")?.GetValue(e)).ToList();
				return await _dbConnection.ExecuteAsync(sql, new { Ids = ids });
			}
			catch (SqlException ex)
			{
				_logger.LogError(ex, "SQL error while deleting multiple records in {TableName}", tableName);
				throw new ApplicationException("Database bulk delete operation failed.", ex);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error in DeleteRangeAsync for {TableName}", tableName);
				throw;
			}
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
