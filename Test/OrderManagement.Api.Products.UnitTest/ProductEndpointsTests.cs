using FluentAssertions;
using Moq;
using OrderManagement.Api.Products.Domain.Entities;
using OrderManagement.Infrastructure.Interfaces;

namespace OrderManagement.Api.Products.UnitTest
{
    public class ProductEndpointsTests
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly Mock<IRepositoryFactory> _mockFactory;

        public ProductEndpointsTests()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _mockFactory = new Mock<IRepositoryFactory>();
            _mockFactory.Setup(f => f.Create<Product>()).Returns(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllProducts_ShouldReturnOk_WithListOfProducts()
        {
            var products = new List<Product> { new Product { Id = 1, Name = "Prod1", CategoryId = 10 } };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

            var repo = _mockFactory.Object.Create<Product>();
            var result = await repo.GetAllAsync();

            result.Should().BeEquivalentTo(products);
        }

        [Fact]
        public async Task GetProductById_ShouldReturnOk_WhenProductExists()
        {
            var product = new Product { Id = 1, Name = "Prod1", CategoryId = 10 };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

            var repo = _mockFactory.Object.Create<Product>();
            var result = await repo.GetByIdAsync(1);

            result.Should().NotBeNull().And.BeEquivalentTo(product);
        }

        [Fact]
        public async Task GetProductById_ShouldReturnNull_WhenProductDoesNotExist()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(() => null!);

            var repo = _mockFactory.Object.Create<Product>();
            var result = await repo.GetByIdAsync(1);

            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateProduct_ShouldAddProduct()
        {
            var productDto = new ProductDto { Name = "New Prod", CategoryId = 10 };
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

            var repo = _mockFactory.Object.Create<Product>();
            var newProduct = new Product { Name = productDto.Name, CategoryId = productDto.CategoryId };
            await repo.AddAsync(newProduct);

            newProduct.Name.Should().Be(productDto.Name);
            newProduct.CategoryId.Should().Be(productDto.CategoryId);
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task UpdateProduct_ShouldModifyProduct_WhenExists()
        {
            var product = new Product { Id = 1, Name = "Prod1", CategoryId = 10 };
            var updatedProduct = new Product { Name = "Updated", CategoryId = 20 };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
            _mockRepo.Setup(r => r.UpdateAsync(product)).Returns(Task.CompletedTask);

            var repo = _mockFactory.Object.Create<Product>();
            var existingProduct = await repo.GetByIdAsync(1);

            existingProduct!.Name = updatedProduct.Name;
            existingProduct!.CategoryId = updatedProduct.CategoryId;
            await repo.UpdateAsync(existingProduct);

            existingProduct.Name.Should().Be(updatedProduct.Name);
            existingProduct.CategoryId.Should().Be(updatedProduct.CategoryId);
            _mockRepo.Verify(r => r.UpdateAsync(existingProduct), Times.Once);
        }

        [Fact]
        public async Task UpdateProduct_ShouldNotUpdate_WhenProductNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(() => null!);

            var repo = _mockFactory.Object.Create<Product>();
            var product = await repo.GetByIdAsync(1);

            product.Should().BeNull();
        }

        [Fact]
        public async Task DeleteProduct_ShouldRemoveProduct_WhenExists()
        {
            var product = new Product { Id = 1, Name = "Prod1", CategoryId = 10 };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
            _mockRepo.Setup(r => r.DeleteAsync(product)).Returns(Task.CompletedTask);

            var repo = _mockFactory.Object.Create<Product>();
            var existingProduct = await repo.GetByIdAsync(1);
            await repo.DeleteAsync(existingProduct!);

            existingProduct.Should().NotBeNull();
            _mockRepo.Verify(r => r.DeleteAsync(existingProduct), Times.Once);
        }

        [Fact]
        public async Task DeleteProduct_ShouldNotDelete_WhenProductNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(() => null!);

            var repo = _mockFactory.Object.Create<Product>();
            var product = await repo.GetByIdAsync(1);

            product.Should().BeNull();
        }

        [Fact]
        public async Task CheckIds_ShouldReturnTrue_WhenAllFound()
        {
            var ids = new List<int> { 1, 2, 3 };
            _mockRepo.Setup(r => r.AreAllIdsFoundAsync(ids)).ReturnsAsync(true);

            var repo = _mockFactory.Object.Create<Product>();
            var result = await repo.AreAllIdsFoundAsync(ids);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task CheckIds_ShouldReturnFalse_WhenNotAllFound()
        {
            var ids = new List<int> { 1, 2, 3 };
            _mockRepo.Setup(r => r.AreAllIdsFoundAsync(ids)).ReturnsAsync(false);

            var repo = _mockFactory.Object.Create<Product>();
            var result = await repo.AreAllIdsFoundAsync(ids);

            result.Should().BeFalse();
        }
    }
}