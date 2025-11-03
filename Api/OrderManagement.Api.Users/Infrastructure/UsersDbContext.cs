using Microsoft.EntityFrameworkCore;
using OrderManagement.Api.Users.Domain.Entities;

namespace OrderManagement.Api.Users.Infrastructure
{
    public class UsersDbContext : DbContext
    {
        public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
    }
}
