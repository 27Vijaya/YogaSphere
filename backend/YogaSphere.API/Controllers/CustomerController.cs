
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YogaSphere.Application.DTOs.Customer;
using YogaSphere.Application.Interfaces;

namespace YogaSphere.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Customer")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    // GET: api/Customer/me
    // Return the profile belonging to the authenticated customer.
    [HttpGet("me")]
    public async Task<ActionResult<CustomerProfileResponse>> GetMyProfile()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var profile = await _customerService.GetProfileAsync(userId);

        if (profile is null)
        {
            return NotFound(new
            {
                message = "Customer profile not found. Please update your profile first."
            });
        }

        return Ok(profile);
    }

    // PUT: api/Customer/me
    // Create or update the authenticated customer's profile.
    [HttpPut("me")]
    public async Task<ActionResult<CustomerProfileResponse>> UpdateMyProfile(
        [FromBody] UpdateCustomerProfileRequest request)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var profile = await _customerService.UpdateProfileAsync(userId, request);

        return Ok(profile);
    }
}
