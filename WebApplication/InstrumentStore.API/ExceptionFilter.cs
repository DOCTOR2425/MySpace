using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InstrumentStore.API
{
	public class ExceptionFilter : IExceptionFilter
	{
		public void OnException(ExceptionContext context)
		{
			try
			{
				// Логируем ошибку
				Console.WriteLine($"Error in {context.ActionDescriptor.DisplayName}: {context.Exception.Message}");

				// Определяем имя контроллера
				var controllerName = context.RouteData.Values["controller"].ToString();

				// Устанавливаем статус-код в зависимости от типа исключения
				var statusCode = context.Exception switch
				{
					NotImplementedException => 501, // Not Implemented
					ArgumentNullException => 400,   // Bad Request
					UnauthorizedAccessException => 401, // Unauthorized
					_ => 500 // Internal Server Error (по умолчанию)
				};

				// Возвращаем JSON с информацией об ошибке
				context.Result = new ObjectResult(new
				{
					Error = context.Exception.Message,
					Controller = controllerName,
					Timestamp = DateTime.UtcNow,
					StatusCode = statusCode // Добавляем статус-код в ответ
				})
				{
					StatusCode = statusCode // Устанавливаем HTTP-код
				};

				// Помечаем исключение как обработанное
				context.ExceptionHandled = true;
			}
			catch (Exception ex)
			{
				// Логируем вторичную ошибку в обработке исключений
				Console.WriteLine($"Exception handling error: {ex.Message}");

				// Возвращаем JSON с информацией о вторичной ошибке
				context.Result = new ObjectResult(new
				{
					Error = "An error occurred while handling another error",
					InnerError = ex.Message,
					Timestamp = DateTime.UtcNow,
					StatusCode = 500 // Internal Server Error для вторичной ошибки
				})
				{
					StatusCode = 500 // Устанавливаем HTTP-код для вторичной ошибки
				};

				// Помечаем вторичное исключение как обработанное
				context.ExceptionHandled = true;
			}
		}
	}
}
