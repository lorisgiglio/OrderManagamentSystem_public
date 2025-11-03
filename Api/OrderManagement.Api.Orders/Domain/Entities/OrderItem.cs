using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Api.Orders.Domain.Entities
{
    [Index(nameof(OrderId), nameof(ProductId), nameof(Quantity), IsUnique = true)]
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
