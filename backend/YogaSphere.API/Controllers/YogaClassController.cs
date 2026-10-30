using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YogaSphere.Application.DTOs.YogaClass;
using YogaSphere.Application.Interfaces;

namespace YogaSphere.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Trainer")]
public class YogaClassController : ControllerBase
{
    private readonly IYogaClassService _yogaClassService;

    public YogaClassController(IYogaClassService yogaClassService)
    {
        _yogaClassService = yogaClassService;
    }

    // Get classes belonging to a trainer's yoga program.
    [HttpGet("program/{programId:int}")]
    public async Task<ActionResult<IReadOnlyList<YogaClassResponse>>> GetByProgramId(
        int programId)
    {
        var trainerIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(trainerIdValue, out var trainerId))
            return Unauthorized();

        var classes = await _yogaClassService.GetByProgramIdAsync(
            trainerId, programId);

        return Ok(classes);
    }

    // Get a specific class owned by the authenticated trainer.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<YogaClassResponse>> GetById(int id)
    {
        var trainerIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(trainerIdValue, out var trainerId))
            return Unauthorized();

        var yogaClass = await _yogaClassService.GetByIdAsync(trainerId, id);

        if (yogaClass is null)
            return NotFound();

        return Ok(yogaClass);
    }

    // Create a class under a program owned by the authenticated trainer.
    [HttpPost]
    public async Task<ActionResult<YogaClassResponse>> Create(
        [FromBody] CreateYogaClassRequest request)
    {
        var trainerIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(trainerIdValue, out var trainerId))
            return Unauthorized();

        var yogaClass = await _yogaClassService.CreateAsync(trainerId, request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = yogaClass.Id },
            yogaClass);
    }

    // Update a class owned by the authenticated trainer.
    [HttpPut("{id:int}")]
    public async Task<ActionResult<YogaClassResponse>> Update(
        int id,
        [FromBody] UpdateYogaClassRequest request)
    {
        var trainerIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(trainerIdValue, out var trainerId))
            return Unauthorized();

        var yogaClass = await _yogaClassService.UpdateAsync(
            trainerId, id, request);

        if (yogaClass is null)
            return NotFound();

        return Ok(yogaClass);
    }

    // Delete a class owned by the authenticated trainer.
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var trainerIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(trainerIdValue, out var trainerId))
            return Unauthorized();

        var deleted = await _yogaClassService.DeleteAsync(trainerId, id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}