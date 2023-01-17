using Exceptions;
using Exceptions.AuthentificationService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

namespace Authentication.API.Middlewares;

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
        catch (AccountCreationException ex)
        {
            await HandleException(httpContext,
                ex,
                StatusCodes.Status400BadRequest,
                "Bad request",
                () => Log.Debug("Creation of account failed: {Message}", GetExceptionDescriptionJson(ex)));
        }
        catch (AccountOperationException ex)
        {
            await HandleException(httpContext,
                ex,
                StatusCodes.Status400BadRequest,
                "Bad request",
                () => Log.Debug(ex.Message, GetExceptionDescriptionJson(ex)));
        }
        catch (ResourceNotFoundException exception)
        {
            await HandleException(httpContext,
                exception,
                StatusCodes.Status404NotFound,
                "Not found",
                () => Log.Debug(exception.Message, GetExceptionDescriptionJson(exception)));
        }
        catch (BadRequestException exception)
        {
            await HandleException(httpContext,
                exception,
                StatusCodes.Status400BadRequest,
                "Bad request",
                () => Log.Debug(exception.Message, GetExceptionDescriptionJson(exception)));
        }
        catch (RefreshTokenException refreshTokenException)
        {
            await HandleException(httpContext,
                refreshTokenException,
                StatusCodes.Status400BadRequest,
                "Bad request",
                () => Log.Debug(refreshTokenException.Message, GetExceptionDescriptionJson(refreshTokenException)));

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
