namespace OrderManagement.Api.Orders.Domain.Entities
{
    public class OrderDto
    {
        public int UserId { get; set; }
        public int DeliveryAddressId { get; set; }
        public ICollection<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }
}
