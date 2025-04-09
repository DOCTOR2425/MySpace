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

            var statusCode = context.Exception switch
            {
                NotImplementedException => 501,
                ArgumentNullException => 400,
                UnauthorizedAccessException => 401,
                InvalidOperationException => 409,
                TimeoutException => 408,
                KeyNotFoundException => 404,
                FormatException => 400,
                DivideByZeroException => 422,
                StackOverflowException => 500,
                OutOfMemoryException => 507,
                AuthenticationException => 403,
                _ => 500
            };


            context.Result = new ObjectResult(new
            {
                Error = context.Exception.Message,
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
