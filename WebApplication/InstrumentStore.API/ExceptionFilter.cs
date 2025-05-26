using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Authentication;

namespace InstrumentStore.API
{
	public class ExceptionFilter : IExceptionFilter
	{
		public void OnException(ExceptionContext context)
		{
			Console.WriteLine($"\nError in {context.ActionDescriptor.DisplayName}:\n{context.Exception.Message}");

			int statusCode = 500;
			string errorMessage = context.Exception.Message;

			statusCode = context.Exception switch
			{
				NotImplementedException => 501,
				ArgumentNullException argNullException => (argNullException.ParamName, 400).Item2,
				ArgumentException => 400,
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
