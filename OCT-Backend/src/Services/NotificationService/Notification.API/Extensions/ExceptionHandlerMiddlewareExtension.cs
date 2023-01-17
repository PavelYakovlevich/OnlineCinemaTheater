using Notification.API.Middlewares;

namespace Notification.API.Extensions;

internal static class ExceptionHandlerMiddlewareExtension
{
    public static void UseExceptionHandler(this WebApplication app) => app.UseMiddleware<ExceptionHandlerMiddleware>();
}
