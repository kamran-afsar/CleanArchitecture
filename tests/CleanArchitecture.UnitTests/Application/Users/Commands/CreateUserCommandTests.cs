using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using FluentAssertions;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Users.Commands;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Infrastructure.Services;
using CleanArchitecture.Tests.Common;

namespace CleanArchitecture.UnitTests.Application.Users.Commands
{
    public class CreateUserCommandTests : TestBase
    {
        private readonly CreateUserCommandHandler _handler;
        private readonly Mock<IPasswordHasher> _mockPasswordHasher;

        public CreateUserCommandTests()
        {
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _handler = new CreateUserCommandHandler(
                MockDbContext.Object,
                _mockPasswordHasher.Object,
                MockEmailService.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldCreateUserAndSendWelcomeEmail()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "Password123!"
            };

            var hashedPassword = "hashedPassword";
            _mockPasswordHasher.Setup(x => x.HashPassword(command.Password))
                .Returns(hashedPassword);

            User savedUser = null;
            MockDbContext.Setup(x => x.Users.Add(It.IsAny<User>()))
                .Callback<User>(user => savedUser = user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBe(Guid.Empty);
            savedUser.Should().NotBeNull();
            savedUser.FirstName.Should().Be(command.FirstName);
            savedUser.LastName.Should().Be(command.LastName);
            savedUser.Email.Should().Be(command.Email);
            savedUser.PasswordHash.Should().Be(hashedPassword);

            MockEmailService.Verify(x => x.SendEmailAsync(
                command.Email,
                It.IsAny<string>(),
                It.IsAny<string>()
            ), Times.Once);

            MockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithExistingEmail_ShouldThrowValidationException()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Email = "existing@example.com"
            };

            var existingUsers = new List<User>
            {
                new User("Existing", "User", "existing@example.com", "hash")
            }.AsQueryable();

            MockDbContext.Setup(x => x.Users)
                .Returns(MockDbSet(existingUsers));

            // Act & Assert
            await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("*Email already exists*");
        }

        private static DbSet<T> MockDbSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return mockSet.Object;
        }
    }
} 