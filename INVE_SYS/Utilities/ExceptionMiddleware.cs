using System.Net;
using System.Text.Json;
using static INVE_SYS.Utilities.Enums;

namespace INVE_SYS.Utilities
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {

            Console.WriteLine($"ExceptionMiddleware triggered for {httpContext.Request.Path}");
            try
            {
                await _next(httpContext);
            }
            catch (CustomException ex)
            {
                await HandleExceptionAsync(httpContext, ex, ex.ClassName ?? "Desconocido", ex.MethodName ?? "Desconocido", ex.Type == ExceptionType.Error);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex,  "Desconocido", "Desconocido", true);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception, string className, string methodName, bool saveInLog)
        {
            var status = exception is CustomException customEx ? customEx.StatusCode : HttpStatusCode.InternalServerError;
            var messageForUser = exception is CustomException customEx2 ? customEx2.UserMessage : "Ha ocurrido un error inesperado. Favor intentarlo más tarde o comunicarse con un administrador.";
            var message = $"{exception.GetType()}. {exception.Message} | {exception.InnerException?.Message}";
            var parametersSent = exception is CustomException customEx3 ? customEx3.ParametersSent : null;
            var type = exception is CustomException customEx4 ? customEx4.Type : ExceptionType.Error;

            if (exception.Message.Contains("BD-CUSTOM-ERROR-ON-RECORD"))
            {
                message = exception.Message.Split('|')[1] ?? "";
                status = HttpStatusCode.BadRequest;
                saveInLog = false;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorResponse
            {
                StatusCode = context.Response.StatusCode,
                Message = messageForUser,
                Type = (int)type
            }));

            _logger.LogError(exception, "An error occurred while processing the request.");
        }
    }
}
