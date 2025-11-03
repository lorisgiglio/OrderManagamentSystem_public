using Microsoft.AspNetCore.Http;

namespace OrderManagement.Infrastructure.Exceptions
{
    /// <summary>
    /// gestisce tutte le eccezioni non catturate che si verificano 
    /// durante l’elaborazione di una richiesta HTTP nella pipeline di ASP.NET Core
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = 500;

            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                detail = exception.Message,
                status = 500
            });

            return context.Response.WriteAsync(result);
        }
    }
}