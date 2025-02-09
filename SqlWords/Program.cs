using System.Data;

using FluentValidation;

using Microsoft.Data.SqlClient;

using SqlWords.Api.Controllers;
using SqlWords.Application.Handlers.Commands.CUD.AddSensitiveWord;
using SqlWords.Application.Handlers.Queries.GetAllSensitiveWords;
using SqlWords.Infrastructure;
using SqlWords.Infrastructure.UnitOfWork;
using SqlWords.Service.Caching.Service;
using SqlWords.Service.Sanitizer.Service;

// Create Builder
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Load Configuration
IConfiguration configuration = builder.Configuration;
builder.Services.AddSingleton(configuration);

// Configure Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

ILogger<Program> _logger = LoggerFactory.Create(logging =>
{
	_ = logging.AddConsole();
	_ = logging.AddDebug();
}).CreateLogger<Program>();

_logger.LogInformation("Starting application setup...");

// Load Connection String
string connectionString = configuration.GetConnectionString("DefaultConnection")
	?? throw new InvalidOperationException("Database connection string is missing.");
_logger.LogInformation("Database connection string loaded.");

// Register Database Connection
builder.Services.AddScoped<IDbConnection>(_ =>
{
	try
	{
		_logger.LogInformation("Initializing database connection...");
		SqlConnection connection = new(connectionString);
		connection.Open();
		_logger.LogInformation("Database connection established.");
		return connection;
	}
	catch (Exception ex)
	{
		_logger.LogCritical(ex, "Failed to establish database connection.");
		throw;
	}
});

// Register Controllers & API Services
builder.Services.AddControllers()
	.AddApplicationPart(typeof(SanitizerController).Assembly)
	.AddApplicationPart(typeof(SensitiveWordController).Assembly)
	.AddControllersAsServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Infrastructure Services
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<DapperUnitOfWork>(sp =>
	new DapperUnitOfWork(connectionString, sp.GetRequiredService<ILogger<DapperUnitOfWork>>()));

// Register MediatR for Application Handlers
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
	typeof(GetAllSensitiveWordsQueryHandler).Assembly,
	typeof(AddSensitiveWordCommandHandler).Assembly
));

// Register FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Register Application Services
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICacheService<string>, WordCacheService>();
builder.Services.AddScoped<ISanitizerService, SanitizerService>();

// Build Application
WebApplication app = builder.Build();
_logger.LogInformation("Application successfully built.");

// Configure Middleware
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Ensure Swagger Works in Development
if (app.Environment.IsDevelopment())
{
	_logger.LogInformation("Enabling Swagger UI for Development.");
	_ = app.UseSwagger();
	_ = app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "SqlWords API v1");
		c.RoutePrefix = "swagger";
	});
}

// Load cache on startup (optional, uncomment if needed)
app.Lifetime.ApplicationStarted.Register(async () =>
{
	using IServiceScope scope = app.Services.CreateScope();
	ICacheService<string> cacheService = scope.ServiceProvider.GetRequiredService<ICacheService<string>>();
	try
	{
		_logger.LogInformation("Refreshing cache on startup...");
		await cacheService.RefreshCacheAsync();
		_logger.LogInformation("Cache successfully refreshed.");
	}
	catch (Exception ex)
	{
		_logger.LogError(ex, "Failed to refresh cache on startup.");
	}
});

// Run Application
try
{
	_logger.LogInformation("Starting application...");
	app.Run();
}
catch (Exception ex)
{
	_logger.LogCritical(ex, "Application encountered a critical failure.");
	throw;
}
