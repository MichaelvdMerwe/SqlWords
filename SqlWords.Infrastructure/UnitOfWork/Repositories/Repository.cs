using System.Data;

using Dapper;

namespace SqlWords.Infrastructure.UnitOfWork.Repositories
{
    public class Repository<T>(IDbConnection dbConnection) : IRepository<T> where T : class
    {
        protected readonly IDbConnection _dbConnection = dbConnection;

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            string sql = $"SELECT * FROM {typeof(T).Name}";
            return await _dbConnection.QueryAsync<T>(sql);
        }

        public async Task<T?> GetAsync(long id)
        {
            string sql = $"SELECT * FROM {typeof(T).Name} WHERE Id = @Id";
            return await _dbConnection.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
        }

        public async Task<int> AddAsync(T entity)
        {
            string sql = GenerateInsertQuery();
            return await _dbConnection.ExecuteAsync(sql, entity);
        }

        public async Task<int> AddRangeAsync(IEnumerable<T> entities)
        {
            string sql = GenerateInsertQuery();
            return await _dbConnection.ExecuteAsync(sql, entities);
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
            string sql = $"DELETE FROM {typeof(T).Name} WHERE Id = @Id";
            return await _dbConnection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<int> DeleteRangeAsync(IEnumerable<T> entities)
        {
            string sql = $"DELETE FROM {typeof(T).Name} WHERE Id IN @Ids";
            object?[] ids = entities.Select(e => e.GetType().GetProperty("Id")?.GetValue(e, null)).ToArray();
            return await _dbConnection.ExecuteAsync(sql, new { Ids = ids });
        }

        private static string GenerateInsertQuery()
        {
            Type type = typeof(T);
            IEnumerable<string> properties = type.GetProperties()
                                 .Where(p => p.Name != "Id")
                                 .Select(p => p.Name);

            string columnNames = string.Join(", ", properties);
            string paramNames = string.Join(", ", properties.Select(p => "@" + p));

            return $"INSERT INTO {type.Name} ({columnNames}) VALUES ({paramNames})";
        }

        private static string GenerateUpdateQuery()
        {
            Type type = typeof(T);
            IEnumerable<string> properties = type.GetProperties()
                                 .Where(p => p.Name != "Id")
                                 .Select(p => $"{p.Name} = @{p.Name}");

            string setClause = string.Join(", ", properties);
            return $"UPDATE {type.Name} SET {setClause} WHERE Id = @Id";
        }
    }
}