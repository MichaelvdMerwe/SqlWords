using System.Data;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SqlWords.Infrastructure.UnitOfWork.Repositories.SensitiveWords;

namespace SqlWords.Infrastructure
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{
			_ = services.AddScoped<IDbConnection>(sp =>
				new SqlConnection(configuration.GetConnectionString("DefaultConnection")));

			_ = services.AddScoped<ISensitiveWordRepository, SensitiveWordRepository>();

			return services;
		}
	}
}