using System.Net;
using static INVE_SYS.Utilities.Enums;

namespace INVE_SYS.Utilities
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public int? Type { get; set; }
    }

    public class CustomException : Exception
    {
        public string? ClassName { get; set; }
        public string? MethodName { get; set; }
        public object? ParametersSent { get; set; }
        public string? UserMessage { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public ExceptionType Type { get; set; } = ExceptionType.Error;

        public CustomException()
        {

        }

        public CustomException(string? message, Exception? innerException, string? userMessage, HttpStatusCode statusCode, string? className = null, string? methodName = null, object? parametersSent = null, ExceptionType type = ExceptionType.Error)
            : base(message, innerException)
        {
            ClassName = className;
            MethodName = methodName;
            StatusCode = statusCode;
            ParametersSent = parametersSent;
            UserMessage = userMessage;
            Type = type;
        }
    }

}
