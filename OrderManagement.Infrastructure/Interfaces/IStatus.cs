using OrderManagement.Infrastructure.Extensions;

namespace OrderManagement.Infrastructure.Interfaces
{
    public interface IStatus
    {
        CurrentStatus Status { get; set; }
    }
}
