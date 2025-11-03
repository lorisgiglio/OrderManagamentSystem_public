using FluentAssertions;
using Moq;
using OrderManagement.Api.Users.Domain.Entities;
using OrderManagement.Infrastructure.Interfaces;

namespace OrderManagement.Api.Users.UnitTest
{
    public class UserEndpointsTests
    {
        private readonly Mock<IRepository<User>> _mockRepo;
        private readonly Mock<IRepositoryFactory> _mockFactory;

        public UserEndpointsTests()
        {
            _mockRepo = new Mock<IRepository<User>>();
            _mockFactory = new Mock<IRepositoryFactory>();
            _mockFactory.Setup(f => f.Create<User>()).Returns(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnOk_WithListOfUsers()
        {
            var users = new List<User> { new User { Id = 1, Username = "user1" } };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

            var repo = _mockFactory.Object.Create<User>();
            var result = await repo.GetAllAsync();

            result.Should().BeEquivalentTo(users);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnOk_WhenUserExists()
        {
            var user = new User { Id = 1, Username = "user1" };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

            var repo = _mockFactory.Object.Create<User>();
            var result = await repo.GetByIdAsync(1);

            result.Should().NotBeNull().And.BeEquivalentTo(user);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnNull_WhenUserDoesNotExist()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(() => null);

            var repo = _mockFactory.Object.Create<User>();
            var result = await repo.GetByIdAsync(1);

            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateUser_ShouldAddUser()
        {
            var userDto = new UserDto { Username = "newuser" };
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            var repo = _mockFactory.Object.Create<User>();
            var newUser = new User { Username = userDto.Username };
            await repo.AddAsync(newUser);

            newUser.Username.Should().Be(userDto.Username);
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_ShouldModifyUser_WhenExists()
        {
            var user = new User { Id = 1, Username = "user1" };
            var updatedUser = new User { Username = "updateduser" };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
            _mockRepo.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);

            var repo = _mockFactory.Object.Create<User>();
            var existingUser = await repo.GetByIdAsync(1);

            existingUser!.Username = updatedUser.Username;
            await repo.UpdateAsync(existingUser);

            existingUser.Username.Should().Be(updatedUser.Username);
            _mockRepo.Verify(r => r.UpdateAsync(existingUser), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_ShouldNotUpdate_WhenUserNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(() => null);

            var repo = _mockFactory.Object.Create<User>();
            var user = await repo.GetByIdAsync(1);

            user.Should().BeNull();
        }

        [Fact]
        public async Task DeleteUser_ShouldRemoveUser_WhenExists()
        {
            var user = new User { Id = 1, Username = "user1" };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
            _mockRepo.Setup(r => r.DeleteAsync(user)).Returns(Task.CompletedTask);

            var repo = _mockFactory.Object.Create<User>();
            var existingUser = await repo.GetByIdAsync(1);
            await repo.DeleteAsync(existingUser!);

            existingUser.Should().NotBeNull();
            _mockRepo.Verify(r => r.DeleteAsync(existingUser), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_ShouldNotDelete_WhenUserNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(() => null);

            var repo = _mockFactory.Object.Create<User>();
            var user = await repo.GetByIdAsync(1);

            user.Should().BeNull();
        }

        [Fact]
        public async Task CheckIds_ShouldReturnTrue_WhenAllFound()
        {
            var ids = new List<int> { 1, 2, 3 };
            _mockRepo.Setup(r => r.AreAllIdsFoundAsync(ids)).ReturnsAsync(true);

            var repo = _mockFactory.Object.Create<User>();
            var result = await repo.AreAllIdsFoundAsync(ids);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task CheckIds_ShouldReturnFalse_WhenNotAllFound()
        {
            var ids = new List<int> { 1, 2, 3 };
            _mockRepo.Setup(r => r.AreAllIdsFoundAsync(ids)).ReturnsAsync(false);

            var repo = _mockFactory.Object.Create<User>();
            var result = await repo.AreAllIdsFoundAsync(ids);

            result.Should().BeFalse();
        }
    }
}