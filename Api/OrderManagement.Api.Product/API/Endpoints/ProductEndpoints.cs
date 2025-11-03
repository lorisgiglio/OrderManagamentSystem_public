using OrderManagement.Api.Products.Domain.Entities;
using OrderManagement.Infrastructure.Interfaces;

namespace OrderManagement.Api.Products.API.Endpoints
{
    public static class ProductEndpoints
    {
        public static void MapProductEndpoints(this WebApplication app)
        {
            app.MapGet("/products", async (IRepositoryFactory factory) =>
            {
                var repo = factory.Create<Product>();
                return Results.Ok(await repo.GetAllAsync());
            });

            app.MapGet("/products/{id}", async (int id, IRepositoryFactory factory) =>
            {
                var repo = factory.Create<Product>();
                var product = await repo.GetByIdAsync(id);
                return product != null ? Results.Ok(product) : Results.NotFound();
            });

            app.MapPost("/products", async (ProductDto product, IRepositoryFactory factory) =>
            {
                // Live Category Check

                var repo = factory.Create<Product>();
                Product p = new() { Name = product.Name, CategoryId = product.CategoryId };
                await repo.AddAsync(p);
                return Results.Created($"/products/{p.Id}", product);
            });

            app.MapPut("/products/{id}", async (int id, Product updatedProduct, IRepositoryFactory factory) =>
            {
                // Live Category Check

                var repo = factory.Create<Product>();
                var product = await repo.GetByIdAsync(id);
                if (product == null) return Results.NotFound();

                product.Name = updatedProduct.Name;
                product.CategoryId = updatedProduct.CategoryId;

                await repo.UpdateAsync(product);
                return Results.NoContent();
            });

            app.MapDelete("/products/{id}", async (int id, IRepositoryFactory factory) =>
            {
                var repo = factory.Create<Product>();
                var product = await repo.GetByIdAsync(id);
                if (product == null) return Results.NotFound();

                await repo.DeleteAsync(product);
                return Results.NoContent();
            });

            app.MapPost("/products/check-ids", async (List<int> ids, IRepositoryFactory factory) =>
            {
                var repo = factory.Create<Product>();
                bool allFound = await repo.AreAllIdsFoundAsync(ids);
                return Results.Ok(allFound);
            });
        }
    }

}
