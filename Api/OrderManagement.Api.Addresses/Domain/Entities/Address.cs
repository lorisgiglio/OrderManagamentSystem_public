using Microsoft.EntityFrameworkCore;
using OrderManagement.Infrastructure.Extensions;
using OrderManagement.Infrastructure.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Api.Addresses.Domain.Entities
{
    [Index(nameof(Street), nameof(City), IsUnique = true)]
    public class Address : IStatus
    {
        [Key]
        public int Id { get; set; }
        public required string Street { get; set; }
        public required string City { get; set; }
        public CurrentStatus Status { get; set; } = CurrentStatus.NotNeeded;
    }
}
