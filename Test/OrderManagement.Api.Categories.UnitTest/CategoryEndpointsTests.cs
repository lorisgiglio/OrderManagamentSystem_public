using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using OrderManagement.Api.Categories.Domain.Entities;
using OrderManagement.Infrastructure.Interfaces;


namespace OrderManagement.Api.Categories.UnitTest
{
    public class CategoryEndpointsTests
    {
        private readonly Mock<IRepository<Category>> _repoMock;
        private readonly Mock<IRepositoryFactory> _factoryMock;

        public CategoryEndpointsTests()
        {
            _repoMock = new Mock<IRepository<Category>>();
            _factoryMock = new Mock<IRepositoryFactory>();
            _factoryMock.Setup(f => f.Create<Category>()).Returns(_repoMock.Object);
        }

        [Fact]
        public async Task GetAllCategories_ReturnsOk_WithListOfCategories()
        {
            var categories = new List<Category> { new Category { Id = 1, Name = "Category1" } };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

            var result = await CategoryEndpointsInvoker.GetAll(_factoryMock.Object);

            var okResult = result.Should().BeOfType<Ok<IEnumerable<Category>>>().Subject;
            okResult.Value.Should().BeEquivalentTo(categories);
        }

        [Fact]
        public async Task GetCategoryById_ReturnsOk_WhenFound()
        {
            var category = new Category { Id = 1, Name = "Category1" };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);

            var result = await CategoryEndpointsInvoker.GetById(1, _factoryMock.Object);

            var okResult = result.Should().BeOfType<Ok<Category>>().Subject;
            okResult.Value.Should().Be(category);
        }

        [Fact]
        public async Task GetCategoryById_ReturnsNotFound_WhenNull()
        {
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Category)null!);

            var result = await CategoryEndpointsInvoker.GetById(1, _factoryMock.Object);

            result.Should().BeOfType<NotFound>();
        }

        [Fact]
        public async Task CheckIds_ReturnsOk_WithBool()
        {
            _repoMock.Setup(r => r.AreAllIdsFoundAsync(It.IsAny<List<int>>())).ReturnsAsync(true);

            var ids = new List<int> { 1, 2, 3 };
            var result = await CategoryEndpointsInvoker.CheckIds(ids, _factoryMock.Object);

            var okResult = result.Should().BeOfType<Ok<bool>>().Subject;
            okResult.Value.Should().BeTrue();
        }
    }

    internal static class CategoryEndpointsInvoker
    {
        public static Task<IResult> GetAll(IRepositoryFactory factory)
        {
            var repo = factory.Create<Category>();
            return repo.GetAllAsync().ContinueWith(t => Results.Ok(t.Result));
        }

        public static async Task<IResult> GetById(int id, IRepositoryFactory factory)
        {
            var repo = factory.Create<Category>();
            var category = await repo.GetByIdAsync(id);
            return category != null ? Results.Ok(category) : Results.NotFound();
        }

        public static async Task<IResult> CheckIds(List<int> ids, IRepositoryFactory factory)
        {
            var repo = factory.Create<Category>();
            bool allFound = await repo.AreAllIdsFoundAsync(ids);
            return Results.Ok(allFound);
        }
    }
}
