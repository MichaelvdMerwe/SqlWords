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

// Load Connection String
string connectionString = configuration.GetConnectionString("DefaultConnection")
	?? throw new InvalidOperationException("Database connection string is missing.");

// Register Database Connection
builder.Services.AddScoped<IDbConnection>(_ => new SqlConnection(connectionString));

// Register Controllers & API Services
builder.Services.AddControllers()
	.AddApplicationPart(typeof(SanitizerController).Assembly)
	.AddApplicationPart(typeof(SensitiveWordController).Assembly)
	.AddControllersAsServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Infrastructure Services
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped(_ => new DapperUnitOfWork(connectionString));

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

// Register Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Build Application
WebApplication app = builder.Build();

// Configure Middleware
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Ensure Swagger Works in Development
if (app.Environment.IsDevelopment())
{
	Console.WriteLine("Executing Swagger Middleware...");
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
	await cacheService.RefreshCacheAsync();
});

// Run Application
app.Run();
