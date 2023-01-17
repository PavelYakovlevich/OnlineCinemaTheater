using User.API.Middlewares;

namespace User.API.Extensions;

internal static class ExceptionHandlerMiddlewareExtension
{
    public static void UseExceptionHandler(this WebApplication app) => app.UseMiddleware<ExceptionHandlerMiddleware>();
}
