using Infrastructure.Consul.Extensions;
using MediaInfo.API.Extensions;
using MediaInfo.API.Middlewares;
using MediaInfo.Data.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();
builder.Host.RegisterInConsul();

builder.SetupMassTransit();
builder.SetupAuthentication();
builder.SetupAuthorization();
builder.SetupFluentValidation();
builder.SetupHealthCheck();
builder.SetupSerilog();
builder.SetupAutoMapper();
builder.SetupDb();
builder.SetupImageService();
builder.SetupParticipantService();
builder.SetupGenreService();
builder.SetupMediaInfoService();

builder.Services.AddControllers()
    .AddNewtonsoftJson();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseHealthChecks("/health");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler();

await app.ApplyMigrations("MediaInfo");
await app.IntializeAzureStorage();
app.Run();
