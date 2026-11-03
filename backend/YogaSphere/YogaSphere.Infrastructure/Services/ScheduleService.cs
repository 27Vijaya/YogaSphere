using YogaSphere.Application.DTOs.Schedule;
using YogaSphere.Application.Interfaces;
using YogaSphere.Application.Interfaces.Repositories;
using YogaSphere.Domain.Entities;

namespace YogaSphere.Infrastructure.Services;

public class ScheduleService : IScheduleService
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IYogaClassRepository _yogaClassRepository;
    private readonly IYogaProgramRepository _yogaProgramRepository;
    private readonly ITrainerProfileRepository _trainerProfileRepository;

    public ScheduleService(
        IScheduleRepository scheduleRepository,
        IYogaClassRepository yogaClassRepository,
        IYogaProgramRepository yogaProgramRepository,
        ITrainerProfileRepository trainerProfileRepository)
    {
        _scheduleRepository = scheduleRepository;
        _yogaClassRepository = yogaClassRepository;
        _yogaProgramRepository = yogaProgramRepository;
        _trainerProfileRepository = trainerProfileRepository;
    }

    // Validate schedule timings.
    private static void ValidateTiming(DateTime startTime, DateTime endTime)
    {
        if (startTime <= DateTime.UtcNow)
            throw new InvalidOperationException(
                "Schedule start time must be in the future.");

        if (endTime <= startTime)
            throw new InvalidOperationException(
                "Schedule end time must be later than start time.");
    }

    // Verify that the class belongs to the authenticated trainer.
    private async Task<YogaClass?> GetOwnedClassAsync(
        int trainerId, int yogaClassId)
    {
        var trainerProfile = await _trainerProfileRepository
            .GetByUserIdAsync(trainerId);

        if (trainerProfile is null)
            return null;

        var yogaClass = await _yogaClassRepository
            .GetByIdAsync(yogaClassId);

        if (yogaClass is null || !yogaClass.IsActive)
            return null;

        var program = await _yogaProgramRepository
            .GetByIdAsync(yogaClass.YogaProgramId);

        if (program is null ||
            !program.IsActive ||
            program.TrainerId != trainerProfile.Id)
            return null;

        return yogaClass;
    }

    // Retrieve schedules belonging to a trainer's yoga class.
    public async Task<IReadOnlyList<ScheduleResponse>> GetByYogaClassIdAsync(
        int trainerId, int yogaClassId)
    {
        var yogaClass = await GetOwnedClassAsync(trainerId, yogaClassId);

        if (yogaClass is null)
            return Array.Empty<ScheduleResponse>();

        var schedules = await _scheduleRepository
            .GetByYogaClassIdAsync(yogaClassId);

        return schedules.Select(MapToResponse).ToList();
    }

    // Retrieve a schedule only if its class belongs to the trainer.
    public async Task<ScheduleResponse?> GetByIdAsync(
        int trainerId, int id)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(id);

        if (schedule is null)
            return null;

        var yogaClass = await GetOwnedClassAsync(
            trainerId, schedule.YogaClassId);

        if (yogaClass is null)
            return null;

        return MapToResponse(schedule);
    }

    // Create a schedule under a class owned by the trainer.
    public async Task<ScheduleResponse> CreateAsync(
        int trainerId, CreateScheduleRequest request)
    {
        var yogaClass = await GetOwnedClassAsync(
            trainerId, request.YogaClassId);

        if (yogaClass is null)
            throw new InvalidOperationException(
                "Yoga class not found or does not belong to this trainer.");

        ValidateTiming(request.StartTime, request.EndTime);

        var hasConflict = await _scheduleRepository.HasConflictAsync(
            request.YogaClassId,
            request.StartTime,
            request.EndTime);

        if (hasConflict)
            throw new InvalidOperationException(
                "This yoga class already has a schedule during the requested time.");

        var schedule = new Schedule
        {
            YogaClassId = request.YogaClassId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _scheduleRepository.AddAsync(schedule);
        await _scheduleRepository.SaveChangesAsync();

        return MapToResponse(schedule);
    }

    // Update a schedule owned by the trainer.
    public async Task<ScheduleResponse?> UpdateAsync(
        int trainerId, int id, UpdateScheduleRequest request)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(id);

        if (schedule is null)
            return null;

        var yogaClass = await GetOwnedClassAsync(
            trainerId, schedule.YogaClassId);

        if (yogaClass is null)
            return null;

        ValidateTiming(request.StartTime, request.EndTime);

        // Check conflicts without comparing the schedule against itself.
        if (request.IsActive)
        {
            var hasConflict = await _scheduleRepository.HasConflictAsync(
                schedule.YogaClassId,
                request.StartTime,
                request.EndTime,
                schedule.Id);

            if (hasConflict)
                throw new InvalidOperationException(
                    "This yoga class already has a schedule during the requested time.");
        }

        schedule.StartTime = request.StartTime;
        schedule.EndTime = request.EndTime;
        schedule.IsActive = request.IsActive;

        await _scheduleRepository.SaveChangesAsync();

        return MapToResponse(schedule);
    }

    // Delete a schedule owned by the trainer.
    public async Task<bool> DeleteAsync(int trainerId, int id)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(id);

        if (schedule is null)
            return false;

        var yogaClass = await GetOwnedClassAsync(
            trainerId, schedule.YogaClassId);

        if (yogaClass is null)
            return false;

        _scheduleRepository.Delete(schedule);
        await _scheduleRepository.SaveChangesAsync();

        return true;
    }

    // Map the database entity to the response DTO.
    private static ScheduleResponse MapToResponse(Schedule schedule)
    {
        return new ScheduleResponse
        {
            Id = schedule.Id,
            YogaClassId = schedule.YogaClassId,
            StartTime = schedule.StartTime,
            EndTime = schedule.EndTime,
            IsActive = schedule.IsActive,
            CreatedAt = schedule.CreatedAt
        };
    }
}