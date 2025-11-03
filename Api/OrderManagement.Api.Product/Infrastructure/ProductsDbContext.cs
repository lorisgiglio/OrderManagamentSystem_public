using Microsoft.EntityFrameworkCore;
using OrderManagement.Api.Products.Domain.Entities;

namespace OrderManagement.Api.Products.Infrastructure
{
    public class ProductsDbContext : DbContext
    {
        public ProductsDbContext(DbContextOptions<ProductsDbContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }
    }
}
