namespace Comment.API.Middlewares;

internal static class ExceptionHandlerMiddlewareExtension
{
    public static void UseExceptionHandler(this WebApplication app) => app.UseMiddleware<ExceptionHandlerMiddleware>();
}
