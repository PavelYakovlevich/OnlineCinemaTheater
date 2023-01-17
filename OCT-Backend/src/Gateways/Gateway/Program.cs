using Gateway.Extensions;
using Ocelot.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddHealthChecks();
builder.ConfigureOcelot();
builder.ConfigureLogger();
builder.ConfigureCORS();

var app = builder.Build();

app.UseCors("allAllowed");

app.UseSerilogRequestLogging();

//app.UseHttpsRedirection();

app.UseHealthChecks("/health");

await app.UseOcelot();

app.Run();
