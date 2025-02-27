using CleanArchitecture.Application.Features.Users.Commands.CreateUser;
using CleanArchitecture.Application.Features.Users.Commands.UpdateUser;
using CleanArchitecture.Application.Features.Users.Queries.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IMediator mediator, ILogger<UsersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(Guid id)
        {
            try
            {
                _logger.LogInformation("Getting user with ID: {UserId}", id);
                var result = await _mediator.Send(new GetUserByIdQuery(id));
                _logger.LogInformation("Successfully retrieved user: {UserId}", id);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found: {UserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user: {UserId}", id);
                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateUser(CreateUserCommand command)
        {
            try
            {
                _logger.LogInformation("Creating new user with email: {Email}", command.Email);
                var result = await _mediator.Send(command);
                _logger.LogInformation("Successfully created user: {UserId}", result);
                return CreatedAtAction(nameof(GetUser), new { id = result }, result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed for create user request");
                return BadRequest(new { errors = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user with email: {Email}", command.Email);
                throw;
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(Guid id, UpdateUserCommand command)
        {
            try
            {
                if (id != command.Id)
                {
                    _logger.LogWarning("ID mismatch in UpdateUser request. Path ID: {PathId}, Command ID: {CommandId}", id, command.Id);
                    return BadRequest(new { message = "ID mismatch" });
                }

                _logger.LogInformation("Updating user: {UserId}", id);
                await _mediator.Send(command);
                _logger.LogInformation("Successfully updated user: {UserId}", id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found for update: {UserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {UserId}", id);
                throw;
            }
        }

        //[HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        //public async Task<ActionResult> DeleteUser(Guid id)
        //{
        //    try
        //    {
        //        _logger.LogInformation("Deleting user: {UserId}", id);
        //        await _mediator.Send(new DeleteUserCommand(id));
        //        _logger.LogInformation("Successfully deleted user: {UserId}", id);
        //        return NoContent();
        //    }
        //    catch (NotFoundException ex)
        //    {
        //        _logger.LogWarning(ex, "User not found for deletion: {UserId}", id);
        //        return NotFound(new { message = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error deleting user: {UserId}", id);
        //        throw;
        //    }
        //}
    }
}