
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YogaSphere.Application.DTOs.Trainer;
using YogaSphere.Application.Interfaces;

namespace YogaSphere.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Trainer")]
public class TrainerController : ControllerBase
{
    private readonly ITrainerService _trainerService;

    public TrainerController(ITrainerService trainerService)
    {
        _trainerService = trainerService;
    }

    // GET: api/Trainer/me
    // Retrieve the authenticated trainer's profile.
    [HttpGet("me")]
    public async Task<ActionResult<TrainerProfileResponse>> GetMyProfile()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var profile = await _trainerService.GetProfileAsync(userId);

        if (profile is null)
        {
            return NotFound(new
            {
                message = "Trainer profile not found. Please update your profile first."
            });
        }

        return Ok(profile);
    }

    // PUT: api/Trainer/me
    // Create or update the authenticated trainer's profile.
    [HttpPut("me")]
    public async Task<ActionResult<TrainerProfileResponse>> UpdateMyProfile(
        [FromBody] UpdateTrainerProfileRequest request)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var profile = await _trainerService.UpdateProfileAsync(userId, request);

        return Ok(profile);
    }
}
