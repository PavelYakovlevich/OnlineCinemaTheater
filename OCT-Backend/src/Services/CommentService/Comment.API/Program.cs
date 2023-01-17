using Comment.API.Middlewares;
using MediaInfo.API.Extensions;
using Comment.Data.Extensions;
using Serilog;
using Infrastructure.Consul.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();
builder.Host.RegisterInConsul();

builder.Services.AddControllers();

builder.SetupAuthorization();
builder.SetupAuthentication();
builder.SetupHeathCheck();
builder.SetupFluentValidation();
builder.SetupServices();
builder.SetupRepositories();
builder.SetupMassTransit();
builder.SetupAutoMapper();
builder.SetupSerilog();

var app = builder.Build();

app.UseExceptionHandler();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseHealthChecks("/health");

app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

await app.EnsureDatabaseCreated("comment", app.Configuration);

app.Run();
