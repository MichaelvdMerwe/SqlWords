using System.Data;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace SqlWords.Infrastructure.UnitOfWork
{
	public class DapperUnitOfWork : IUnitOfWork, IDisposable
	{
		private readonly ILogger<DapperUnitOfWork> _logger;
		public IDbConnection Connection { get; }
		public IDbTransaction? Transaction { get; private set; }

		public DapperUnitOfWork(string connectionString, ILogger<DapperUnitOfWork> logger)
		{
			_logger = logger;
			try
			{
				Connection = new SqlConnection(connectionString);
				Connection.Open();
				Transaction = Connection.BeginTransaction();

				_logger.LogInformation("DapperUnitOfWork started. Transaction initialized.");
			}
			catch (SqlException ex)
			{
				_logger.LogError(ex, "Database connection failed: {Message}", ex.Message);
				throw new ApplicationException("Failed to establish database connection.", ex);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error initializing DapperUnitOfWork.");
				throw;
			}
		}

		public void Complete()
		{
			try
			{
				Transaction?.Commit();
				_logger.LogInformation("Transaction committed successfully.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error committing transaction.");
				throw new ApplicationException("Failed to commit transaction.", ex);
			}
			finally
			{
				DisposeTransaction();
			}
		}

		public void Rollback()
		{
			try
			{
				Transaction?.Rollback();
				_logger.LogWarning("Transaction rolled back.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error rolling back transaction.");
				throw new ApplicationException("Failed to rollback transaction.", ex);
			}
			finally
			{
				DisposeTransaction();
			}
		}

		private void DisposeTransaction()
		{
			if (Transaction != null)
			{
				Transaction.Dispose();
				Transaction = null;
				_logger.LogInformation("Transaction disposed.");
			}
		}

		//implement async transaction handling

		public void Dispose()
		{
			DisposeTransaction();

			if (Connection != null)
			{
				try
				{
					Connection.Dispose();
					_logger.LogInformation("Database connection disposed.");
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error disposing database connection.");
				}
			}

			GC.SuppressFinalize(this);
		}
	}
}
