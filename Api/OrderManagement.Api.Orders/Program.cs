using Microsoft.Extensions.Options;
using OrderManagement.Api.Orders.API.Endpoints;
using OrderManagement.Api.Orders.Application;
using OrderManagement.Api.Orders.Infrastructure;
using OrderManagement.Application.Extensions;
using OrderManagement.Infrastructure.Configurations;
using OrderManagement.Infrastructure.Extensions;
using OrderManagement.Infrastructure.HttpValidation;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddCommonApiServices<OrdersDbContext>(
    builder.Configuration,
    "Order Management API - ORDER",
    "v1",
    "SqliteConnection"
);

// Hosted Service per la validazione asincrona degli ordini
builder.Services.AddHostedService<ValidationBackgroundService>();
builder.Services.Configure<ValidationEndpointsOptions>(builder.Configuration.GetSection("ValidationEndpoints"));
builder.Services.AddTransient<HttpValidation>();
builder.Services.AddHttpClient("UsersClient", (sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<ValidationEndpointsOptions>>().Value;
    client.BaseAddress = new Uri(options.UsersBaseUrl);
});

builder.Services.AddHttpClient("AddressesClient", (sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<ValidationEndpointsOptions>>().Value;
    client.BaseAddress = new Uri(options.AddressesBaseUrl);
});

builder.Services.AddHttpClient("ProductsClient", (sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<ValidationEndpointsOptions>>().Value;
    client.BaseAddress = new Uri(options.ProductsBaseUrl);
});


var app = builder.Build();
app.UseCommonApiSetup(appBuilder => appBuilder.MapOrderEndpoints());

app.RunMigrations<OrdersDbContext>(builder.Configuration);

app.Run();

