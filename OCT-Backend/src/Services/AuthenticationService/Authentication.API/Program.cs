using Authentication.API.Extensions;
using Infrastructure.Consul.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.RegisterInConsul();

builder.Services.AddControllers();

builder.SetupHealthCheck();
builder.SetupDbContext();
builder.SetupLogger();
builder.SetupMassTransit();
builder.SetupAutoMapper();
builder.SetupApplicationConfiguration();
builder.SetupAuthService();
builder.SetupFluentValidation();

builder.Host.UseSerilog();

var app = builder.Build();

app.UseExceptionHandler();

app.UseSerilogRequestLogging();

//app.UseHttpsRedirection();

app.UseHealthChecks("/health");

app.UseAuthorization();

app.MapControllers();

app.ApplyMigrations();

app.Run();
