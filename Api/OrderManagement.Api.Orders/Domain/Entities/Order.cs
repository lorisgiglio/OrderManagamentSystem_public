using OrderManagement.Infrastructure.Extensions;
using OrderManagement.Infrastructure.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Api.Orders.Domain.Entities
{
    public class Order : IStatus
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int DeliveryAddressId { get; set; }
        public ICollection<OrderItem> Items { get; set; } = [];
        public CurrentStatus Status { get; set; } = CurrentStatus.Draft;
    }

}
