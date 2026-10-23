
using Microsoft.AspNetCore.Mvc;
using YogaSphere.Application.DTOs.User;
using YogaSphere.Application.Interfaces.Repositories;
using YogaSphere.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace YogaSphere.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll()
    {
        var users = await _userRepository.GetAllAsync();

        var response = users.Select(user => new UserResponseDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email
        });

        return Ok(response);
    }


    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<UserResponseDto>> GetById(int id)
    {
        // Get the authenticated user's ID from the JWT.
        var currentUserId = User.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var isAdmin = User.IsInRole("Admin");

        // Customers can access only their own user record.
        if (!isAdmin &&
            (!int.TryParse(currentUserId, out var authenticatedUserId) ||
             authenticatedUserId != id))
        {
            return Forbid();
        }

        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
            return NotFound();

        return Ok(ToResponseDto(user));
    }



    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserResponseDto>> GetMyProfile()
    {
        // Read the authenticated user's ID from the JWT token.
        var currentUserId = User.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        // Reject requests if the token does not contain a valid user ID.
        if (!int.TryParse(currentUserId, out var authenticatedUserId))
        {
            return Unauthorized();
        }

        // Fetch only the authenticated user's own record.
        var user = await _userRepository.GetByIdAsync(authenticatedUserId);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(ToResponseDto(user));
    }



    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserResponseDto>> Update(
        int id,
        CreateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
            return NotFound();

        var email = dto.Email.Trim();
        var existingUser = await _userRepository.GetByEmailAsync(email);

        if (existingUser is not null && existingUser.Id != id)
            return Conflict(new { message = "Email is already registered." });

        user.FirstName = dto.FirstName.Trim();
        user.LastName = dto.LastName.Trim();
        user.Email = email;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        return Ok(ToResponseDto(user));
    }



    [HttpPut("me")]
    [Authorize]
    public async Task<ActionResult<UserResponseDto>> UpdateMyProfile(
        UpdateUserDto dto)
    {
        // Get the authenticated user's ID from the JWT token.
        var currentUserId = User.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        // Reject requests if the token does not contain a valid user ID.
        if (!int.TryParse(currentUserId, out var authenticatedUserId))
        {
            return Unauthorized();
        }

        // Load only the authenticated user's record.
        var user = await _userRepository.GetByIdAsync(authenticatedUserId);

        if (user is null)
        {
            return NotFound();
        }

        // Normalize the email before checking for duplicates.
        var email = dto.Email.Trim().ToLowerInvariant();

        // Prevent the user from taking another account's email address.
        var existingUser = await _userRepository.GetByEmailAsync(email);

        if (existingUser is not null && existingUser.Id != user.Id)
        {
            return Conflict(new
            {
                message = "Email is already registered."
            });
        }

        // Update only the fields permitted by UpdateUserDto.
        user.FirstName = dto.FirstName.Trim();
        user.LastName = dto.LastName.Trim();
        user.Email = email;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        return Ok(ToResponseDto(user));
    }



    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
            return NotFound();

        _userRepository.Delete(user);
        await _userRepository.SaveChangesAsync();

        return NoContent();
    }

    private static UserResponseDto ToResponseDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email
        };
    }
}
