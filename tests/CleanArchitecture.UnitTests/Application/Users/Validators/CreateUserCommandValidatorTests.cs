using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using CleanArchitecture.Application.Users.Commands;
using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.UnitTests.Application.Users.Validators
{
    public class CreateUserCommandValidatorTests
    {
        private readonly CreateUserCommandValidator _validator;
        private readonly Mock<IApplicationDbContext> _mockDbContext;

        public CreateUserCommandValidatorTests()
        {
            _mockDbContext = new Mock<IApplicationDbContext>();
            _validator = new CreateUserCommandValidator(_mockDbContext.Object);
        }

        [Theory]
        [InlineData("", "Invalid FirstName")]
        [InlineData("A", "FirstName too short")]
        [InlineData("AB123", "FirstName contains invalid characters")]
        public async Task Validate_InvalidFirstName_ShouldHaveValidationError(string firstName, string expectedError)
        {
            // Arrange
            var command = new CreateUserCommand
            {
                FirstName = firstName,
                LastName = "Doe",
                Email = "john@example.com",
                Password = "Password123!"
            };

            // Act
            var result = await _validator.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage.Contains(expectedError));
        }

        [Theory]
        [InlineData("test", "Password too short")]
        [InlineData("password", "Password must contain at least one uppercase letter")]
        [InlineData("Password", "Password must contain at least one number")]
        public async Task Validate_InvalidPassword_ShouldHaveValidationError(string password, string expectedError)
        {
            // Arrange
            var command = new CreateUserCommand
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = password
            };

            // Act
            var result = await _validator.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.ErrorMessage.Contains(expectedError));
        }

        [Fact]
        public async Task Validate_ValidCommand_ShouldNotHaveValidationError()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "Password123!"
            };

            // Act
            var result = await _validator.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }
    }
} 