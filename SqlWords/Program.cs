using System.Data;

using Microsoft.Data.SqlClient;

using SqlWords.Application.Handlers.Commands.CUD.AddSensitiveWord;
using SqlWords.Application.Handlers.Queries.GetAllSensitiveWords;
using SqlWords.Infrastructure;
using SqlWords.Service.Caching.Service;
using SqlWords.Service.Sanitizer.Service;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Ensure Configuration is registered
IConfiguration configuration = builder.Configuration;
builder.Services.AddSingleton(configuration);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);

// Register MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
	typeof(GetAllSensitiveWordsQueryHandler).Assembly,
	typeof(AddSensitiveWordCommandHandler).Assembly
));

// Register Services
builder.Services.AddScoped<ICacheService<string>, WordCacheService>();
builder.Services.AddScoped<ISanitizerService, SanitizerService>();

// Load Connection String
string connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Database connection string is missing."); ;

// Ensure database connection is configured in Infrastructure
builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(connectionString));

WebApplication app = builder.Build();

// Load cache on startup
using (IServiceScope scope = app.Services.CreateScope())
{
	ICacheService<string> cacheService = scope.ServiceProvider.GetRequiredService<ICacheService<string>>();
	await cacheService.RefreshCacheAsync(); // Preload cache
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
	_ = app.UseSwagger();
	_ = app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "SqlWords API v1");
		c.RoutePrefix = string.Empty;  // Swagger available at root URL
	});
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
