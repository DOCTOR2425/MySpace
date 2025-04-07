using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InstrumentStore.API
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            Console.WriteLine($"Error in {context.ActionDescriptor.DisplayName}: {context.Exception.Message}");

            var controllerName = context.RouteData.Values["controller"].ToString();

            var statusCode = context.Exception switch
            {
                NotImplementedException => 501,
                ArgumentNullException => 400,
                UnauthorizedAccessException => 401,
                _ => 500
            };

            context.Result = new ObjectResult(new
            {
                Error = context.Exception.Message,
                Controller = controllerName,
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
