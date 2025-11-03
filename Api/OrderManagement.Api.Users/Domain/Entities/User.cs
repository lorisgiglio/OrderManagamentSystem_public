using Microsoft.EntityFrameworkCore;
using OrderManagement.Infrastructure.Extensions;
using OrderManagement.Infrastructure.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Api.Users.Domain.Entities
{
    [Index(nameof(Username), IsUnique = true)]
    public class User : IStatus
    {
        [Key]
        public int Id { get; set; }
        public required string Username { get; set; }
        public CurrentStatus Status { get; set; } = CurrentStatus.NotNeeded;
    }

}
