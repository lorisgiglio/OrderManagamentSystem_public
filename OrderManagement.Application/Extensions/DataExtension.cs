using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OrderManagement.Application.Extensions
{
    public static class DataExtension
    {
        public static WebApplication RunMigrations<TDbContext>(
                    this WebApplication app,
                    IConfiguration configuration)
                    where TDbContext : DbContext
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
                dbContext.Database.Migrate();
            }
            return app;
        }
    }
}
