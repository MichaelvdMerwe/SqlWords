﻿using System.ComponentModel.DataAnnotations.Schema;
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

		public async Task<int> AddRangeAsync(IEnumerable<T> entities)
		{
			if (!entities.Any())
			{
				return 0;
			}

			string sql = Repository<T>.GenerateInsertQuery();

			try
			{
				using (IDbTransaction transaction = _dbConnection.BeginTransaction())
				{
					int rowsAffected = await _dbConnection.ExecuteAsync(sql, entities, transaction);
					transaction.Commit();
					return rowsAffected;
				}
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
			if (!entities.Any())
			{
				return 0;
			}

			string sql = Repository<T>.GenerateUpdateQuery();
			int rowsAffected = 0;

			try
			{
				using (IDbTransaction transaction = _dbConnection.BeginTransaction())
				{
					foreach (T entity in entities)
					{
						rowsAffected += await _dbConnection.ExecuteAsync(sql, entity, transaction);
					}
					transaction.Commit();
				}

				return rowsAffected;
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
			if (!entities.Any())
			{
				return 0;
			}

			string tableName = GetTableName();
			IEnumerable<long> ids = entities
				.Select(e => e.GetType().GetProperty("Id")?.GetValue(e))
				.Where(id => id != null)
				.Cast<long>();

			if (!ids.Any())
			{
				return 0;
			}

			string sql = $"DELETE FROM {tableName} WHERE Id IN ({string.Join(",", ids)})";

			try
			{
				using (IDbTransaction transaction = _dbConnection.BeginTransaction())
				{
					int rowsAffected = await _dbConnection.ExecuteAsync(sql, transaction);
					transaction.Commit();
					return rowsAffected;
				}
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
				.Where(p => p.Name != "Id")
				.Select(p => $"[{p.Name}]");

			string columnNames = string.Join(", ", properties);
			string paramNames = string.Join(", ", properties.Select(p => $"@{p.Replace("[", "").Replace("]", "")}"));

			return $"INSERT INTO {tableName} ({columnNames}) VALUES ({paramNames})";
		}

		private static string GenerateUpdateQuery()
		{
			Type type = typeof(T);
			string tableName = GetTableName();

			IEnumerable<string> properties = type.GetProperties()
				.Where(p => p.Name != "Id")
				.Select(p => $"[{p.Name}] = @{p.Name}");

			string setClause = string.Join(", ", properties);

			return $"UPDATE {tableName} SET {setClause} WHERE [Id] = @Id";
		}

		private static string GetTableName()
		{
			Type type = typeof(T);
			string tableName = type.GetCustomAttributes(typeof(TableAttribute), false)
								   .FirstOrDefault() is TableAttribute tableAttribute
				? tableAttribute.Name
				: type.Name;

			return $"dbo.[{tableName}]";
		}

		Task<List<long>> IRepository<T>.AddRangeAsync(IEnumerable<T> entities)
		{
			throw new NotImplementedException();
		}
	}
}
