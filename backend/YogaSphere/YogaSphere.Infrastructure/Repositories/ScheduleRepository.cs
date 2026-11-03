using Microsoft.EntityFrameworkCore;
using YogaSphere.Application.Interfaces.Repositories;
using YogaSphere.Domain.Entities;
using YogaSphere.Infrastructure.Persistence;

namespace YogaSphere.Infrastructure.Repositories;

public class ScheduleRepository : IScheduleRepository
{
    private readonly YogaSphereDbContext _context;

    public ScheduleRepository(YogaSphereDbContext context)
    {
        _context = context;
    }

    // Retrieve schedules belonging to a specific yoga class.
    public async Task<IReadOnlyList<Schedule>> GetByYogaClassIdAsync(
        int yogaClassId)
    {
        return await _context.Schedules
            .AsNoTracking()
            .Where(schedule => schedule.YogaClassId == yogaClassId)
            .OrderBy(schedule => schedule.StartTime)
            .ToListAsync();
    }

    // Retrieve a schedule by its ID.
    public async Task<Schedule?> GetByIdAsync(int id)
    {
        return await _context.Schedules
            .FirstOrDefaultAsync(schedule => schedule.Id == id);
    }

    // Add a new schedule.
    public async Task AddAsync(Schedule schedule)
    {
        await _context.Schedules.AddAsync(schedule);
    }

    // Check whether another schedule overlaps the requested time.
    public async Task<bool> HasConflictAsync(
        int yogaClassId,
        DateTime startTime,
        DateTime endTime,
        int? excludeScheduleId = null)
    {
        return await _context.Schedules.AnyAsync(schedule =>
            schedule.YogaClassId == yogaClassId &&
            schedule.IsActive &&
            schedule.StartTime < endTime &&
            schedule.EndTime > startTime &&
            (!excludeScheduleId.HasValue ||
             schedule.Id != excludeScheduleId.Value));
    }

    // Mark a schedule for deletion.
    public void Delete(Schedule schedule)
    {
        _context.Schedules.Remove(schedule);
    }

    // Persist pending changes to SQL Server.
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}