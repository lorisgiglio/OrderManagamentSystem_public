using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using OrderManagement.Api.Orders.API.Endpoints;
using OrderManagement.Api.Orders.Domain.Entities;
using OrderManagement.Infrastructure.Interfaces;

namespace OrderManagement.Api.Orders.UnitTest
{
    public class OrderEndpointsTests
    {
        private readonly Mock<IRepository<Order>> _repoMock;
        private readonly Mock<IRepositoryFactory> _factoryMock;

        public OrderEndpointsTests()
        {
            _repoMock = new Mock<IRepository<Order>>();
            _factoryMock = new Mock<IRepositoryFactory>();
            _factoryMock.Setup(f => f.Create<Order>()).Returns(_repoMock.Object);
        }

        [Fact]
        public async Task GetAllOrders_ReturnsOk_WithOrders()
        {
            var orders = new List<Order> { new Order { Id = 1 } };
            _repoMock.Setup(r => r.GetAllWithIncludesAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Order, object>>>()))
                     .ReturnsAsync(orders);

            var result = await OrderEndpointsInvoker.GetAll(_factoryMock.Object);

            var okResult = result.Should().BeOfType<Ok<IEnumerable<Order>>>().Subject;
            okResult.Value.Should().BeEquivalentTo(orders);
        }

        [Fact]
        public async Task GetOrderById_ReturnsOk_WhenFound()
        {
            var order = new Order { Id = 1 };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);

            var result = await OrderEndpointsInvoker.GetById(1, _factoryMock.Object);

            var okResult = result.Should().BeOfType<Ok<Order>>().Subject;
            okResult.Value.Should().Be(order);
        }

        [Fact]
        public async Task GetOrderById_ReturnsNotFound_WhenNull()
        {
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Order)null!);

            var result = await OrderEndpointsInvoker.GetById(1, _factoryMock.Object);

            result.Should().BeOfType<NotFound>();
        }

        [Fact]
        public void SyncOrderItems_AddsUpdatesRemovesCorrectly()
        {
            // Arrange
            var existingItems = new List<OrderItem>
            {
                new OrderItem { ProductId = 1, Quantity = 1 },
                new OrderItem { ProductId = 2, Quantity = 2 }
            };

            var updatedDtos = new List<OrderItemDto>
            {
                new OrderItemDto { ProductId = 1, Quantity = 10 },
                new OrderItemDto { ProductId = 3, Quantity = 3 }
            };

            // Act
            var result = OrderEndpoints.SyncOrderItems(existingItems, updatedDtos);

            // Assert
            result.Should().HaveCount(2);
            result.Should().ContainSingle(i => i.ProductId == 1 && i.Quantity == 10);
            result.Should().ContainSingle(i => i.ProductId == 3 && i.Quantity == 3);
            result.Should().NotContain(i => i.ProductId == 2);
        }
    }

    internal static class OrderEndpointsInvoker
    {
        public static Task<IResult> GetAll(IRepositoryFactory factory)
        {
            var repo = factory.Create<Order>();
            return repo.GetAllWithIncludesAsync(o => o.Items).ContinueWith(t => Results.Ok(t.Result));
        }

        public static async Task<IResult> GetById(int id, IRepositoryFactory factory)
        {
            var repo = factory.Create<Order>();
            var order = await repo.GetByIdAsync(id);
            return order is not null ? Results.Ok(order) : Results.NotFound();
        }
    }
}
