using YogaSphere.Application.DTOs.Schedule;

namespace YogaSphere.Application.Interfaces;

public interface IScheduleService
{
    // Retrieve schedules belonging to a trainer's yoga class.
    Task<IReadOnlyList<ScheduleResponse>> GetByYogaClassIdAsync(int trainerId, int yogaClassId);

    // Retrieve active, upcoming schedules for customers.
    Task<IReadOnlyList<ScheduleResponse>> GetAvailableByYogaClassIdAsync(int yogaClassId);

    // Retrieve a schedule by its ID.
    Task<ScheduleResponse?> GetByIdAsync(int trainerId, int id);

    // Create a schedule under a class owned by the trainer.
    Task<ScheduleResponse> CreateAsync(int trainerId, CreateScheduleRequest request);

    // Update a schedule owned by the trainer.
    Task<ScheduleResponse?> UpdateAsync( int trainerId,int id,UpdateScheduleRequest request);

    // Delete a schedule owned by the trainer.
    Task<bool> DeleteAsync(int trainerId, int id);
}