using System;
using Xunit;
using FluentAssertions;
using CleanArchitecture.Application.Users.Events;

namespace CleanArchitecture.UnitTests.Domain.Entities
{
    public class UserTests
    {
        [Fact]
        public void CreateUser_WithValidData_ShouldCreateUserAndRaiseDomainEvent()
        {
            // Arrange
            var firstName = "John";
            var lastName = "Doe";
            var email = "john@example.com";
            var passwordHash = "hashedPassword";

            // Act
            var user = new User(firstName, lastName, email, passwordHash);

            // Assert
            user.FirstName.Should().Be(firstName);
            user.LastName.Should().Be(lastName);
            user.Email.Should().Be(email);
            user.PasswordHash.Should().Be(passwordHash);
            user.DomainEvents.Should().ContainSingle(e => e is UserCreatedEvent);
        }

        [Fact]
        public void UpdateUser_WithValidData_ShouldUpdateUserAndRaiseDomainEvent()
        {
            // Arrange
            var user = new User("John", "Doe", "john@example.com", "hash");
            var newFirstName = "Jane";
            var newLastName = "Smith";
            var newEmail = "jane@example.com";

            // Act
            user.Update(newFirstName, newLastName, newEmail);

            // Assert
            user.FirstName.Should().Be(newFirstName);
            user.LastName.Should().Be(newLastName);
            user.Email.Should().Be(newEmail);
            user.DomainEvents.Should().Contain(e => e is UserUpdatedEvent);
        }
    }
} 