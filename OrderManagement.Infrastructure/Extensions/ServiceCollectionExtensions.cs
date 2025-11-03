using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using OrderManagement.Infrastructure.Interfaces;

namespace OrderManagement.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommonApiServices<TContext>(
            this IServiceCollection services,
            IConfiguration configuration,
            string title,
            string version,
            string connectionStringName)
            where TContext : DbContext
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(version, new OpenApiInfo { Title = title, Version = version });
            });

            services.AddDbContext<TContext>(options =>
                options.UseSqlite(configuration.GetConnectionString(connectionStringName))
                .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning)));

            services.AddScoped<IRepositoryFactory, RepositoryFactory<TContext>>();

            return services;
        }
    }
}
