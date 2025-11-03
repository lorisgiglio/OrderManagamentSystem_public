using OrderManagement.Api.Addresses.API.Endpoints;
using OrderManagement.Api.Addresses.Infrastructure;
using OrderManagement.Application.Extensions;
using OrderManagement.Infrastructure.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddCommonApiServices<AddressesDbContext>(
    builder.Configuration,
    "Order Management API - ADDRESS",
    "v1",
    "SqliteConnection"
);

var app = builder.Build();
app.UseCommonApiSetup(appBuilder => appBuilder.MapAddressEndpoints());
app.RunMigrations<AddressesDbContext>(builder.Configuration);

app.Run();
