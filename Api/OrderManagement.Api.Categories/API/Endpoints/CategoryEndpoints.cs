using OrderManagement.Api.Categories.Domain.Entities;
using OrderManagement.Infrastructure.Interfaces;

namespace OrderManagement.Api.Categories.API.Endpoints
{
    public static class CategoryEndpoints
    {
        public static void MapCategoryEndpoints(this WebApplication app)
        {
            app.MapGet("/categories", async (IRepositoryFactory factory) =>
            {
                var repo = factory.Create<Category>();
                return Results.Ok(await repo.GetAllAsync());
            });

            app.MapGet("/categories/{id}", async (int id, IRepositoryFactory factory) =>
            {
                var repo = factory.Create<Category>();
                var category = await repo.GetByIdAsync(id);
                return category != null ? Results.Ok(category) : Results.NotFound();
            });

            app.MapPost("/categories", async (CategoryDto category, IRepositoryFactory factory) =>
            {
                var repo = factory.Create<Category>();
                Category c = new() { Name = category.Name };
                await repo.AddAsync(c);
                return Results.Created($"/categories/{c.Id}", category);
            });

            app.MapPut("/categories/{id}", async (int id, CategoryDto updatedCategory, IRepositoryFactory factory) =>
            {
                var repo = factory.Create<Category>();
                var category = await repo.GetByIdAsync(id);
                if (category == null) return Results.NotFound();

                category.Name = updatedCategory.Name;

                await repo.UpdateAsync(category);
                return Results.NoContent();
            });

            app.MapDelete("/categories/{id}", async (int id, IRepositoryFactory factory) =>
            {
                var repo = factory.Create<Category>();
                var category = await repo.GetByIdAsync(id);
                if (category == null) return Results.NotFound();

                await repo.DeleteAsync(category);
                return Results.NoContent();
            });

            app.MapPost("/categories/check-ids", async (List<int> ids, IRepositoryFactory factory) =>
            {
                var repo = factory.Create<Category>();
                bool allFound = await repo.AreAllIdsFoundAsync(ids);
                return Results.Ok(allFound);
            });
        }
    }
}
