using Microsoft.EntityFrameworkCore;
using OrderManagement.Api.Addresses.Domain.Entities;

namespace OrderManagement.Api.Addresses.Infrastructure
{
    public class AddressesDbContext : DbContext
    {
        public AddressesDbContext(DbContextOptions<AddressesDbContext> options) : base(options) { }
        public DbSet<Address> Addresses { get; set; }
    }
}
