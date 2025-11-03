using OrderManagement.Api.Addresses.Domain.Entities;
using OrderManagement.Infrastructure.Interfaces;

namespace OrderManagement.Api.Addresses.API.Endpoints
{
    public static class AddressEndpoints
    {
        public static void MapAddressEndpoints(this WebApplication app)
        {
            app.MapGet("/addresses", async (IRepositoryFactory factory) =>
            {
                var repo = factory.Create<Address>();
                return Results.Ok(await repo.GetAllAsync());
            });

            app.MapGet("/addresses/{id}", async (int id, IRepositoryFactory factory) =>
            {
                var repo = factory.Create<Address>();
                var address = await repo.GetByIdAsync(id);
                return address != null ? Results.Ok(address) : Results.NotFound();
            });

            app.MapPost("/addresses", async (AddressDto address, IRepositoryFactory factory) =>
            {
                var repo = factory.Create<Address>();
                Address a = new() { City = address.City, Street = address.Street };
                await repo.AddAsync(a);
                return Results.Created($"/addresses/{a.Id}", address);
            });

            app.MapPut("/addresses/{id}", async (int id, AddressDto updatedAddress, IRepositoryFactory factory) =>
            {
                var repo = factory.Create<Address>();
                var address = await repo.GetByIdAsync(id);
                if (address == null) return Results.NotFound();

                address.Street = updatedAddress.Street;
                address.City = updatedAddress.City;

                await repo.UpdateAsync(address);
                return Results.NoContent();
            });

            app.MapDelete("/addresses/{id}", async (int id, IRepositoryFactory factory) =>
            {
                var repo = factory.Create<Address>();
                var address = await repo.GetByIdAsync(id);
                if (address == null) return Results.NotFound();

                await repo.UpdateAsync(address);
                return Results.NoContent();
            });

            app.MapPost("/addresses/check-ids", async (List<int> ids, IRepositoryFactory factory) =>
            {
                var repo = factory.Create<Address>();
                bool allFound = await repo.AreAllIdsFoundAsync(ids);
                return Results.Ok(allFound);
            });
        }
    }

}
