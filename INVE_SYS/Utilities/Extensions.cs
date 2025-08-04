using Microsoft.AspNetCore.StaticFiles;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using static INVE_SYS.Utilities.Enums;

namespace INVE_SYS.Utilities
{
    public static class Extensions
    {
        public static string NameOf(this object o)
        {
            return o.GetType().Name;
        }

        public static string GetCaller([CallerMemberName] string caller = "")
        {
            return caller;
        }

        public static CustomException TransformException(Exception ex, string defaultUserMessage, object parameters, string className, string methodName)
        {
            if (ex is CustomException customEx)
            {
                return new CustomException(
                    ex.Message,
                    customEx.InnerException ?? ex,
                    customEx.UserMessage ?? defaultUserMessage,
                    customEx.StatusCode,
                    customEx.ClassName ?? className,
                    customEx.MethodName ?? methodName,
                    parameters,
                    customEx.Type
                );
            }

            return new CustomException(
                ex.Message,
                ex,
                defaultUserMessage,
                HttpStatusCode.BadRequest,
                className,
                methodName,
                parameters,
                ExceptionType.Error
            );
        }


    }
}
