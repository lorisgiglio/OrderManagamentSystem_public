using Microsoft.EntityFrameworkCore;
using OrderManagement.Infrastructure.Extensions;
using OrderManagement.Infrastructure.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Api.Categories.Domain.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Category : IStatus
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public CurrentStatus Status { get; set; } = CurrentStatus.NotNeeded;
    }
}
