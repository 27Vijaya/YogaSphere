using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YogaSphere.Application.DTOs.Schedule;
using YogaSphere.Application.Interfaces;

namespace YogaSphere.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Trainer")]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _scheduleService;

    public ScheduleController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    // Get schedules belonging to a trainer's yoga class.
    [HttpGet("class/{yogaClassId:int}")]
    public async Task<ActionResult<IReadOnlyList<ScheduleResponse>>> GetByYogaClassId(
        int yogaClassId)
    {
        var trainerIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(trainerIdValue, out var trainerId))
            return Unauthorized();

        var schedules = await _scheduleService.GetByYogaClassIdAsync(
            trainerId, yogaClassId);

        return Ok(schedules);
    }

    // Get a specific schedule owned by the authenticated trainer.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ScheduleResponse>> GetById(int id)
    {
        var trainerIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(trainerIdValue, out var trainerId))
            return Unauthorized();

        var schedule = await _scheduleService.GetByIdAsync(trainerId, id);

        if (schedule is null)
            return NotFound();

        return Ok(schedule);
    }

    // Create a schedule under a class owned by the trainer.
    [HttpPost]
    public async Task<ActionResult<ScheduleResponse>> Create(
        [FromBody] CreateScheduleRequest request)
    {
        var trainerIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(trainerIdValue, out var trainerId))
            return Unauthorized();

        try
        {
            var schedule = await _scheduleService.CreateAsync(
                trainerId, request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = schedule.Id },
                schedule);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Update a schedule owned by the authenticated trainer.
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ScheduleResponse>> Update(
        int id,
        [FromBody] UpdateScheduleRequest request)
    {
        var trainerIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(trainerIdValue, out var trainerId))
            return Unauthorized();

        try
        {
            var schedule = await _scheduleService.UpdateAsync(
                trainerId, id, request);

            if (schedule is null)
                return NotFound();

            return Ok(schedule);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Delete a schedule owned by the authenticated trainer.
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var trainerIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(trainerIdValue, out var trainerId))
            return Unauthorized();

        var deleted = await _scheduleService.DeleteAsync(trainerId, id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}