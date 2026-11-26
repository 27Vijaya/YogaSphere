
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YogaSphere.Application.DTOs.YogaClass;
using YogaSphere.Application.Interfaces;

namespace YogaSphere.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class YogaClassController : ControllerBase
{
    private readonly IYogaClassService _yogaClassService;

    public YogaClassController(IYogaClassService yogaClassService)
    {
        _yogaClassService = yogaClassService;
    }

    // Customers can view active classes.
    // Trainers can view classes belonging to their own programs.
    [HttpGet("program/{programId:int}")]
    public async Task<ActionResult<IReadOnlyList<YogaClassResponse>>>
        GetByProgramId(int programId)
    {
        if (User.IsInRole("Trainer"))
        {
            var trainerIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(trainerIdValue, out var trainerId))
                return Unauthorized();

            var trainerClasses = await _yogaClassService
                .GetByProgramIdAsync(trainerId, programId);

            return Ok(trainerClasses);
        }

        if (User.IsInRole("Customer") || User.IsInRole("Admin"))
        {
            var availableClasses = await _yogaClassService
                .GetAvailableByProgramIdAsync(programId);

            return Ok(availableClasses);
        }

        return Forbid();
    }

    // Customers can view active classes.
    // Trainers can view only their own classes.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<YogaClassResponse>> GetById(int id)
    {
        if (User.IsInRole("Trainer"))
        {
            var trainerIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(trainerIdValue, out var trainerId))
                return Unauthorized();

            var trainerClass = await _yogaClassService
                .GetByIdAsync(trainerId, id);

            if (trainerClass is null)
                return NotFound();

            return Ok(trainerClass);
        }

        if (User.IsInRole("Customer") || User.IsInRole("Admin"))
        {
            var availableClass = await _yogaClassService
                .GetAvailableByIdAsync(id);

            if (availableClass is null)
                return NotFound();

            return Ok(availableClass);
        }

        return Forbid();
    }

    // Only Trainers and Admins can create classes.
    [Authorize(Roles = "Trainer,Admin")]
    [HttpPost]
    public async Task<ActionResult<YogaClassResponse>> Create(
        [FromBody] CreateYogaClassRequest request)
    {
        var trainerIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(trainerIdValue, out var trainerId))
            return Unauthorized();

        var yogaClass = await _yogaClassService
            .CreateAsync(trainerId, request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = yogaClass.Id },
            yogaClass);
    }

    // Only Trainers and Admins can update classes.
    [Authorize(Roles = "Trainer,Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<YogaClassResponse>> Update(
        int id,
        [FromBody] UpdateYogaClassRequest request)
    {
        var trainerIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(trainerIdValue, out var trainerId))
            return Unauthorized();

        var yogaClass = await _yogaClassService
            .UpdateAsync(trainerId, id, request);

        if (yogaClass is null)
            return NotFound();

        return Ok(yogaClass);
    }

    // Only Trainers and Admins can delete classes.
    [Authorize(Roles = "Trainer,Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var trainerIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(trainerIdValue, out var trainerId))
            return Unauthorized();

        var deleted = await _yogaClassService
            .DeleteAsync(trainerId, id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
