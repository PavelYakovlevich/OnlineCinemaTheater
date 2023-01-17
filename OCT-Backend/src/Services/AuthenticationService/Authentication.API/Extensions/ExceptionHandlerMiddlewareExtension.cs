using Authentication.API.Middlewares;

namespace Authentication.API.Extensions;

internal static class ExceptionHandlerMiddlewareExtension
{
    public static void UseExceptionHandler(this WebApplication app) => app.UseMiddleware<ExceptionHandlerMiddleware>();
}
