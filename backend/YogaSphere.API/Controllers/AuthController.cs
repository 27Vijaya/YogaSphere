
using Microsoft.AspNetCore.Mvc;
using YogaSphere.Application.DTOs.Auth;
using YogaSphere.Application.Interfaces;
using YogaSphere.Application.Exceptions;

namespace YogaSphere.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register(RegisterRequest request)
    {
        try
        {
            var user = await _authService.RegisterAsync(request);

            // Return 201 Created and provide the location of the new user.
            return Created($"/api/Users/{user.Id}", user);
        }
        catch (DuplicateEmailException ex)
        {
            // A duplicate email is a client conflict, not a server failure.
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);

        if (response is null)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        return Ok(response);
    }
}
