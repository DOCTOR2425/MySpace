using System.Security.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InstrumentStore.API
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            Console.WriteLine($"\nError in {context.ActionDescriptor.DisplayName}:\n{context.Exception.Message}");

            int statusCode = 500;
            string errorMessage = context.Exception.Message;

            switch (context.Exception)
            {
                case NotImplementedException:
                    statusCode = 501;
                    break;
                case ArgumentNullException argNullException:
                    statusCode = 400;
                    errorMessage = argNullException.ParamName;
                    break;
                case UnauthorizedAccessException:
                    statusCode = 401;
                    break;
                case InvalidOperationException:
                    statusCode = 409;
                    break;
                case TimeoutException:
                    statusCode = 408;
                    break;
                case KeyNotFoundException:
                    statusCode = 404;
                    break;
                case FormatException:
                    statusCode = 400;
                    break;
                case DivideByZeroException:
                    statusCode = 422;
                    break;
                case StackOverflowException:
                    statusCode = 500;
                    break;
                case OutOfMemoryException:
                    statusCode = 507;
                    break;
                case AuthenticationException:
                    statusCode = 403;
                    break;
                default:
                    statusCode = 500;
                    break;
            }

            context.Result = new ObjectResult(new
            {
                Error = errorMessage,
                Timestamp = DateTime.UtcNow,
                StatusCode = statusCode
            })
            {
                StatusCode = statusCode
            };

            context.ExceptionHandled = true;
        }
    }
}
