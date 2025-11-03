using Microsoft.EntityFrameworkCore;
using OrderManagement.Api.Categories.Domain.Entities;

namespace OrderManagement.Api.Categories.Infrastructure
{
    public class CategoriesDbContext : DbContext
    {
        public CategoriesDbContext(DbContextOptions<CategoriesDbContext> options) : base(options) { }
        public DbSet<Category> Categories { get; set; }
    }
}
