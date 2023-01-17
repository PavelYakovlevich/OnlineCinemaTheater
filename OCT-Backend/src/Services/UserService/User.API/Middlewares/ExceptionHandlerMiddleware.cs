using Exceptions.UserService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

namespace User.API.Middlewares;

internal class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next.Invoke(httpContext);
        }
        catch (BlobOperationException exception)
        {
            await HandleException(httpContext,
                exception,
                StatusCodes.Status400BadRequest,
                "Bad request",
                () => Log.Error("Operation with blob failed: {Message}", GetExceptionDescriptionJson(exception)));
        }
        catch (UserOperationException exception)
        {
            await HandleException(httpContext,
                exception,
                StatusCodes.Status400BadRequest,
                "Bad request",
                () => Log.Error("Operation with user failed: {Message}", GetExceptionDescriptionJson(exception)));
        }
        catch (Exception exception)
        {
            await HandleException(httpContext,
                exception,
                StatusCodes.Status500InternalServerError,
                "Internal error",
                () => Log.Error("Execution failed with message: {Message}", GetExceptionDescriptionJson(exception)));
        }

        static string GetExceptionDescriptionJson(Exception exception) =>
            JsonConvert.SerializeObject(new { exception.Message, exception.StackTrace },
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                });
    }

    private async Task HandleException(HttpContext context, Exception exception, int statusCode, string statusMessage, Action logAction)
    {
        logAction();

        var response = context.Response;
        response.ContentType = "application/json";
        response.StatusCode = statusCode;
        var allMessageText = GetFullMessage(exception);

        await response.WriteAsync(JsonConvert.SerializeObject(new ProblemDetails
        {
            Detail = allMessageText,
            Status = statusCode,
            Title = statusMessage,
            Instance = context.Request.Path,
        }));
    }

    private string GetFullMessage(Exception ex)
    {
        if (ex.InnerException != null)
        {
            return ex.Message + "; " + GetFullMessage(ex.InnerException);
        }

        return ex.Message;
    }
}
