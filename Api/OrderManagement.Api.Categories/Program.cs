using OrderManagement.Api.Categories.API.Endpoints;
using OrderManagement.Api.Categories.Infrastructure;
using OrderManagement.Application.Extensions;
using OrderManagement.Infrastructure.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddCommonApiServices<CategoriesDbContext>(
    builder.Configuration,
    "Order Management API - CATEGORY",
    "v1",
    "SqliteConnection"
);

var app = builder.Build();
app.UseCommonApiSetup(appBuilder => appBuilder.MapCategoryEndpoints());

app.RunMigrations<CategoriesDbContext>(builder.Configuration);

app.Run();

