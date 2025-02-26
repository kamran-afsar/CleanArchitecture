using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using CleanArchitecture.Application.Users.Queries;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Application.Common.Mappings;
using CleanArchitecture.Application.Common.TestHelpers;

namespace CleanArchitecture.UnitTests.Application.Users.Queries
{
    public class GetUserDetailsQueryTests : TestBase
    {
        private readonly GetUserDetailsQueryHandler _handler;

        public GetUserDetailsQueryTests()
        {
            _handler = new GetUserDetailsQueryHandler(
                MockUserRepository.Object,
                Mapper);
        }

        [Fact]
        public async Task Handle_WithValidId_ShouldReturnUserDetails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userDetails = new UserDetailDto
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Roles = new List<string> { "User" }
            };

            MockUserRepository.Setup(x => x.GetUserDetailsByIdAsync(userId))
                .ReturnsAsync(userDetails);

            var query = new GetUserDetailsQuery(userId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(userId);
            result.FirstName.Should().Be(userDetails.FirstName);
            result.LastName.Should().Be(userDetails.LastName);
            result.Email.Should().Be(userDetails.Email);
            result.Roles.Should().BeEquivalentTo(userDetails.Roles);
        }

        [Fact]
        public async Task Handle_WithInvalidId_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            MockUserRepository.Setup(x => x.GetUserDetailsByIdAsync(userId))
                .ReturnsAsync((UserDetailDto)null);

            var query = new GetUserDetailsQuery(userId);

            // Act & Assert
            await _handler.Invoking(x => x.Handle(query, CancellationToken.None))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Entity \"User\" ({userId}) was not found.");
        }
    }
} 