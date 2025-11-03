using Microsoft.EntityFrameworkCore;
using OrderManagement.Infrastructure.Extensions;
using OrderManagement.Infrastructure.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Api.Products.Domain.Entities
{
    [Index(nameof(Name), nameof(CategoryId), IsUnique = true)]
    public class Product : IStatus
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public required int CategoryId { get; set; }
        public CurrentStatus Status { get; set; } = CurrentStatus.Draft;
    }
}
