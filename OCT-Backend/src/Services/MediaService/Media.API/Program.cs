using Infrastructure.Consul.Extensions;
using Media.API.Extensions;
using Media.API.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();
builder.Host.RegisterInConsul();

builder.Services.AddControllers();

builder.SetupAuthentication();
builder.SetupAuthorization();
builder.SetupAutoMapper();
builder.SetupServices();
builder.SetupDb();
builder.SetupSerilog();
builder.SetupHealthCheck();
builder.SetupMassTransit();
builder.SetupBlobStorage();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.MapControllers();

app.UseExceptionHandler();

app.UseHealthChecks("/health");

app.UseAuthentication();
app.UseAuthorization();

await app.IntializeAzureStorage();
await app.ApplyMigrations();

app.Run();
