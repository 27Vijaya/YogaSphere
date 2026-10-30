
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YogaSphere.Application.DTOs.YogaProgram;
using YogaSphere.Application.Interfaces;

namespace YogaSphere.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Trainer")]
public class YogaProgramController : ControllerBase
{
    private readonly IYogaProgramService _yogaProgramService;

    public YogaProgramController(IYogaProgramService yogaProgramService)
    {
        _yogaProgramService = yogaProgramService;
    }

    // Get all yoga programs.
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<YogaProgramResponse>>> GetAll()
    {
        var programs = await _yogaProgramService.GetAllAsync();
        return Ok(programs);
    }

    // Get a specific yoga program by ID.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<YogaProgramResponse>> GetById(int id)
    {
        var program = await _yogaProgramService.GetByIdAsync(id);

        if (program is null)
        {
            return NotFound();
        }

        return Ok(program);
    }

    // Create a yoga program for the authenticated trainer.
    [HttpPost]
    public async Task<ActionResult<YogaProgramResponse>> Create(
        [FromBody] CreateYogaProgramRequest request)
    {
        var trainerIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(trainerIdValue, out var trainerId))
        {
            return Unauthorized();
        }

        var program = await _yogaProgramService.CreateAsync(trainerId, request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = program.Id },
            program);
    }

    // Update only a program owned by the authenticated trainer.
    [HttpPut("{id:int}")]
    public async Task<ActionResult<YogaProgramResponse>> Update(
        int id,
        [FromBody] UpdateYogaProgramRequest request)
    {
        var trainerIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(trainerIdValue, out var trainerId))
        {
            return Unauthorized();
        }

        var program = await _yogaProgramService.UpdateAsync(
            trainerId, id, request);

        // A missing program or one owned by another trainer is not found.
        if (program is null)
        {
            return NotFound();
        }

        return Ok(program);
    }

    // Delete only a program owned by the authenticated trainer.
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var trainerIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(trainerIdValue, out var trainerId))
        {
            return Unauthorized();
        }

        var deleted = await _yogaProgramService.DeleteAsync(trainerId, id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
