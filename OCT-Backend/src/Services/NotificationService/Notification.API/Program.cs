using NLog.Web;
using Notification.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.SetupHealthCheck();
builder.SetupAutoMapper();
builder.SetupNotificationService();
builder.SetupMassTransit();

builder.Host.UseNLog();

var app = builder.Build();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseHealthChecks("/health");

app.Run();