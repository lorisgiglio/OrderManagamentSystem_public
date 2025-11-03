using Microsoft.Extensions.Options;
using OrderManagement.Api.Products.API.Endpoints;
using OrderManagement.Api.Products.Application;
using OrderManagement.Api.Products.Infrastructure;
using OrderManagement.Application.Extensions;
using OrderManagement.Infrastructure.Configurations;
using OrderManagement.Infrastructure.Extensions;
using OrderManagement.Infrastructure.HttpValidation;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddCommonApiServices<ProductsDbContext>(
    builder.Configuration,
    "Order Management API - PRODUCT",
    "v1",
    "SqliteConnection"
);
builder.Services.AddHostedService<ValidationBackgroundService>();
builder.Services.Configure<ValidationEndpointsOptions>(builder.Configuration.GetSection("ValidationEndpoints"));
builder.Services.AddTransient<HttpValidation>();
builder.Services.AddHttpClient("CategoriesClient", (sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<ValidationEndpointsOptions>>().Value;
    client.BaseAddress = new Uri(options.CategoriesBaseUrl);
});


var app = builder.Build();
app.UseCommonApiSetup(appBuilder => appBuilder.MapProductEndpoints());


app.RunMigrations<ProductsDbContext>(builder.Configuration);

app.Run();
