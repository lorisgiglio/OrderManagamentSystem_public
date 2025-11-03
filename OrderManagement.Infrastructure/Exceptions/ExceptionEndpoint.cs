using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace OrderManagement.Infrastructure.Exceptions
{
    public static class ExceptionEndpoint
    {
        public static void MapGenericExceptionEndpoint(this WebApplication app)
        {
            // Gestione globale delle eccezioni (in aggiunta si può avere una classe che implementi IExceptionHandler)
            app.UseExceptionHandler("/error");

            app.Map("/error", (HttpContext httpContext) =>
            {
                var exceptionHandlerPathFeature = httpContext.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature?.Error;

                return Results.Problem(detail: exception?.Message, statusCode: 500);
            });

        }
    }
}
