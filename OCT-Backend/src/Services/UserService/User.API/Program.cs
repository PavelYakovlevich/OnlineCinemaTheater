using Infrastructure.Consul.Extensions;
using Serilog;
using User.API.Extensions;
using User.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();
builder.Host.RegisterInConsul();

builder.Services.AddControllers();

builder.SetupHealthCheck();
builder.SetupFluentValidation();
builder.SetupUserService();
builder.SetupImageService();
builder.SetupMassTransit();
builder.SetupSerilog();
builder.SetupDb();
builder.SetupAutoMapper();
builder.SetupAuthentication();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseHealthChecks("/health");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.ApplyMigrations("Users");
await app.IntializeAzureStorage();

app.Run();
