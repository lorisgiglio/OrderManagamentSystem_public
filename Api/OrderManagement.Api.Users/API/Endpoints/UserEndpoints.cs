using OrderManagement.Api.Users.Domain.Entities;
using OrderManagement.Infrastructure.Interfaces;

namespace OrderManagement.Api.Users.API.Endpoints
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this WebApplication app)
        {
            app.MapGet("/users", async (IRepositoryFactory factory) =>
            {
                var repo = factory.Create<User>();
                return Results.Ok(await repo.GetAllAsync());
            });

            app.MapGet("/users/{id}", async (int id, IRepositoryFactory factory) =>
            {
                var repo = factory.Create<User>();
                var user = await repo.GetByIdAsync(id);
                return user != null ? Results.Ok(user) : Results.NotFound();
            });

            app.MapPost("/users", async (UserDto user, IRepositoryFactory factory) =>
            {
                var repo = factory.Create<User>();
                User u = new() { Username = user.Username };
                await repo.AddAsync(u);
                return Results.Created($"/users/{u.Id}", user);
            });

            app.MapPut("/users/{id}", async (int id, User updatedUser, IRepositoryFactory factory) =>
            {
                var repo = factory.Create<User>();
                var user = await repo.GetByIdAsync(id);
                if (user == null) return Results.NotFound();

                user.Username = updatedUser.Username;

                await repo.UpdateAsync(user);
                return Results.NoContent();
            });

            app.MapDelete("/users/{id}", async (int id, IRepositoryFactory factory) =>
            {
                var repo = factory.Create<User>();
                var user = await repo.GetByIdAsync(id);
                if (user == null) return Results.NotFound();

                await repo.DeleteAsync(user);
                return Results.NoContent();
            });

            app.MapPost("/users/check-ids", async (List<int> ids, IRepositoryFactory factory) =>
            {
                var repo = factory.Create<User>();
                bool allFound = await repo.AreAllIdsFoundAsync(ids);
                return Results.Ok(allFound);
            });
        }
    }

}
