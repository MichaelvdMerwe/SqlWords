using System.Data;

using Microsoft.Data.SqlClient;

namespace SqlWords.Infrastructure.UnitOfWork
{
	public class DapperUnitOfWork : IUnitOfWork, IDisposable
	{
		public IDbConnection Connection { get; }
		public IDbTransaction? Transaction { get; private set; }

		public DapperUnitOfWork(string connectionString)
		{
			Connection = new SqlConnection(connectionString);
			Connection.Open();
			Transaction = Connection.BeginTransaction();
		}

		public void Complete()
		{
			Transaction?.Commit();
			Transaction?.Dispose();
			Transaction = null;
		}

		public void Rollback()
		{
			Transaction?.Rollback();
			Transaction?.Dispose();
			Transaction = null;
		}

		public void Dispose()
		{
			Transaction?.Dispose();
			Connection?.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
