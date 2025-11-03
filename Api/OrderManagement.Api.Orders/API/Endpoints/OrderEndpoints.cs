using OrderManagement.Api.Orders.Domain.Entities;
using OrderManagement.Infrastructure.Extensions;
using OrderManagement.Infrastructure.Interfaces;

namespace OrderManagement.Api.Orders.API.Endpoints
{
    public static class OrderEndpoints
    {
        public static void MapOrderEndpoints(this WebApplication app)
        {
            app.MapGet("/orders", async (IRepositoryFactory factory) =>
            {
                var repo = factory.Create<Order>();
                return Results.Ok(await repo.GetAllWithIncludesAsync(o => o.Items));
            });

            app.MapGet("/orders/{id}", async (int id, IRepositoryFactory factory) =>
            {
                var repo = factory.Create<Order>();
                var order = await repo.GetByIdAsync(id);
                return order is not null ? Results.Ok(order) : Results.NotFound();
            });

            app.MapPost("/orders", async (OrderDto order, IRepositoryFactory factory) =>
            {
                var repo = factory.Create<Order>();
                Order o = new()
                {
                    UserId = order.UserId,
                    DeliveryAddressId = order.DeliveryAddressId,
                    Items = [.. order.Items.Select(i => new OrderItem
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity
                    })],
                    Status = CurrentStatus.Draft
                };
                await repo.AddAsync(o);
                return Results.Created($"/orders/{o.Id}", order);
            });

            app.MapPut("/orders/{id}", async (int id, OrderDto updatedOrder, IRepositoryFactory factory) =>
            {
                var repo = factory.Create<Order>();
                var order = await repo.GetByIdWithIncludesAsync(id, o => o.Items);

                if (order is null) return Results.NotFound();

                order.UserId = updatedOrder.UserId;
                order.DeliveryAddressId = updatedOrder.DeliveryAddressId;
                order.Items = SyncOrderItems(order.Items, updatedOrder.Items);

                await repo.UpdateAsync(order);
                return Results.NoContent();
            });

            app.MapDelete("/orders/{id}", async (int id, IRepositoryFactory factory) =>
            {
                var repo = factory.Create<Order>();
                var order = await repo.GetByIdAsync(id);
                if (order is null) return Results.NotFound();

                await repo.DeleteAsync(order);
                return Results.NoContent();
            });
        }

        public static ICollection<OrderItem> SyncOrderItems(ICollection<OrderItem> orderToUpdate, ICollection<OrderItemDto> newItems)
        {
            if (orderToUpdate == null)
                return [.. newItems.Select(x => x.ToOrderItem())];

            if (newItems == null || newItems.Count == 0)
            {
                orderToUpdate.Clear();
                return orderToUpdate;
            }

            var newItemsLookup = newItems.ToDictionary(dto => dto.ProductId);
            var productIdsToRemove = new List<int>();

            // Update items
            foreach (var existingItem in orderToUpdate.ToList())
            {
                if (newItemsLookup.TryGetValue(existingItem.ProductId, out var newItemDto))
                {
                    existingItem.Quantity = newItemDto.Quantity;
                    newItemsLookup.Remove(existingItem.ProductId);
                }
                else
                {
                    productIdsToRemove.Add(existingItem.ProductId);
                }
            }

            var itemsToRemove = orderToUpdate
                .Where(item => productIdsToRemove.Contains(item.ProductId))
                .ToList();

            // Remove items
            foreach (var item in itemsToRemove)
            {
                orderToUpdate.Remove(item);
            }

            // Add new items
            foreach (var dtoToAdd in newItemsLookup.Values)
            {
                var newItem = new OrderItemDto
                {
                    ProductId = dtoToAdd.ProductId,
                    Quantity = dtoToAdd.Quantity
                };

                orderToUpdate.Add(newItem.ToOrderItem());
            }

            return orderToUpdate;
        }
    }
}