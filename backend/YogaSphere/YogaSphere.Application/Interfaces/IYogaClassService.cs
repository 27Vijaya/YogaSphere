using YogaSphere.Application.DTOs.YogaClass;

namespace YogaSphere.Application.Interfaces;

public interface IYogaClassService
{
    // Retrieve classes belonging to a yoga program.
    Task<IReadOnlyList<YogaClassResponse>> GetByProgramIdAsync(
        int trainerId,
        int yogaProgramId);

    // Retrieve a specific class.
    Task<YogaClassResponse?> GetByIdAsync( int trainerId, int id);

    // Create a class under a program owned by the trainer.
    Task<YogaClassResponse> CreateAsync( int trainerId,CreateYogaClassRequest request);

    // Update a class owned by the trainer.
    Task<YogaClassResponse?> UpdateAsync(int trainerId, int id, UpdateYogaClassRequest request);

    // Delete a class owned by the trainer.
    Task<bool> DeleteAsync(int trainerId, int id);
}