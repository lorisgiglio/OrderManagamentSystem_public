using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using OrderManagement.Infrastructure.Exceptions;

namespace OrderManagement.Infrastructure.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseCommonApiSetup(this WebApplication app, Action<WebApplication> mapCustomEndpoints)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Chiama il delegato per mappare gli endpoint specifici (ad es. order endpoints)
            mapCustomEndpoints(app);

            // Gestione eccezioni generica
            app.MapGenericExceptionEndpoint();

            // Middleware gestione globale eccezioni
            app.UseExceptionHandling();

            return app;
        }
    }
}
