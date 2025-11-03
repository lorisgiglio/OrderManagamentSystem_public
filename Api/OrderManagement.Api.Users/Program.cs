using OrderManagement.Api.Users.API.Endpoints;
using OrderManagement.Api.Users.Infrastructure;
using OrderManagement.Application.Extensions;
using OrderManagement.Infrastructure.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddCommonApiServices<UsersDbContext>(
    builder.Configuration,
    "Order Management API - USER",
    "v1",
    "SqliteConnection"
);

var app = builder.Build();
app.UseCommonApiSetup(appBuilder => appBuilder.MapUserEndpoints());

app.RunMigrations<UsersDbContext>(builder.Configuration);

app.Run();
