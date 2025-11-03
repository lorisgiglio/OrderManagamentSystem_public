using Microsoft.EntityFrameworkCore;
using OrderManagement.Api.Orders.Domain.Entities;

namespace OrderManagement.Api.Orders.Infrastructure
{
    public class OrdersDbContext : DbContext
    {
        public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options) { }
        public DbSet<Order> Orders { get; set; }
    }
}
