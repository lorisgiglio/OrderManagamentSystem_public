namespace OrderManagement.Api.Orders.Domain.Entities
{
    public static class MappingExtensions
    {
        public static OrderItem ToOrderItem(this OrderItemDto dto)
        {
            if (dto == null)
            {
                return null!;
            }

            return new OrderItem
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
            };
        }

        public static IEnumerable<OrderItem> ToOrderItems(this IEnumerable<OrderItemDto> dtos)
        {
            if (dtos == null)
            {
                return [];
            }
            return dtos.Select(dto => dto.ToOrderItem());
        }
    }

}
