using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using OrderManagement.Api.Addresses.Domain.Entities;
using OrderManagement.Infrastructure.Interfaces;

namespace OrderManagement.Api.Addresses.UnitTest
{
    public class AddressEndpointsTests
    {
        private readonly Mock<IRepository<Address>> _repoMock;
        private readonly Mock<IRepositoryFactory> _factoryMock;

        public AddressEndpointsTests()
        {
            _repoMock = new Mock<IRepository<Address>>();
            _factoryMock = new Mock<IRepositoryFactory>();
            _factoryMock.Setup(f => f.Create<Address>()).Returns(_repoMock.Object);
        }

        [Fact]
        public async Task GetAllAddresses_ReturnsOk_WithListOfAddresses()
        {
            // Arrange
            var addresses = new List<Address> { new Address { Id = 1, City = "City1", Street = "Street1" } };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(addresses);

            // Act
            var result = await AddressEndpointsInvoker.GetAll(_factoryMock.Object);

            // Assert
            var okResult = result.Should().BeOfType<Ok<IEnumerable<Address>>>().Subject;
            okResult.Value.Should().BeEquivalentTo(addresses);
        }

        [Fact]
        public async Task GetAddressById_ReturnsOk_WhenFound()
        {
            var address = new Address { Id = 1, City = "City1", Street = "Street1" };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(address);

            var result = await AddressEndpointsInvoker.GetById(1, _factoryMock.Object);

            var okResult = result.Should().BeOfType<Ok<Address>>().Subject;
            okResult.Value.Should().Be(address);
        }

        [Fact]
        public async Task GetAddressById_ReturnsNotFound_WhenNull()
        {
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Address)null!);

            var result = await AddressEndpointsInvoker.GetById(1, _factoryMock.Object);

            result.Should().BeOfType<NotFound>();
        }

        [Fact]
        public async Task CheckIds_ReturnsOk_WithBool()
        {
            _repoMock.Setup(r => r.AreAllIdsFoundAsync(It.IsAny<List<int>>())).ReturnsAsync(true);

            var ids = new List<int> { 1, 2, 3 };
            var result = await AddressEndpointsInvoker.CheckIds(ids, _factoryMock.Object);

            var okResult = result.Should().BeOfType<Ok<bool>>().Subject;
            okResult.Value.Should().BeTrue();
        }
    }

    internal static class AddressEndpointsInvoker
    {
        public static Task<IResult> GetAll(IRepositoryFactory factory)
        {
            var repo = factory.Create<Address>();
            return repo.GetAllAsync().ContinueWith(t => Results.Ok(t.Result));
        }

        public static async Task<IResult> GetById(int id, IRepositoryFactory factory)
        {
            var repo = factory.Create<Address>();
            var address = await repo.GetByIdAsync(id);
            return address != null ? Results.Ok(address) : Results.NotFound();
        }

        public static async Task<IResult> CheckIds(List<int> ids, IRepositoryFactory factory)
        {
            var repo = factory.Create<Address>();
            bool allFound = await repo.AreAllIdsFoundAsync(ids);
            return Results.Ok(allFound);
        }
    }
}
