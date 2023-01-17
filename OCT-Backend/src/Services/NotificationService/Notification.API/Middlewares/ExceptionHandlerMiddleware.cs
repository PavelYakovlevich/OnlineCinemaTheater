using Exceptions.NotificationService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Notification.API.Middlewares
{
    internal class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<Exception> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext);
            }
            catch (EmailSendInternalException ex)
            {
                await HandleException(httpContext,
                    ex,
                    StatusCodes.Status500InternalServerError,
                    "Internal error",
                    () => _logger.LogError("Sending of email failed with message: {Message}", GetExceptionDescriptionJson(ex)));
            }
            catch (EmailSendUserException ex)
            {
                await HandleException(httpContext,
                    ex,
                    StatusCodes.Status400BadRequest,
                    "Bad request",
                    () => _logger.LogError("Sending of email failed with message: {Message}", GetExceptionDescriptionJson(ex)));
            }
            catch (Exception exception)
            {
                await HandleException(httpContext,
                    exception,
                    StatusCodes.Status500InternalServerError,
                    "Internal error",
                    () => _logger.LogError("Execution failed with message: {Message}", GetExceptionDescriptionJson(exception)));
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
}
