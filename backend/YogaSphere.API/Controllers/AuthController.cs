
using Microsoft.AspNetCore.Mvc;
using YogaSphere.Application.DTOs.Auth;
using YogaSphere.Application.Interfaces;

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
    public async Task<ActionResult<UserResponse>> Register(
        RegisterRequest request)
    {
        try
        {
            var user = await _authService.RegisterAsync(request);

            return Created($"/api/Users/{user.Id}", user);
        }
        catch (InvalidOperationException ex)
        {
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
