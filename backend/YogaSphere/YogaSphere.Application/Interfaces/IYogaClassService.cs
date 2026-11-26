
using YogaSphere.Application.DTOs.YogaClass;

namespace YogaSphere.Application.Interfaces;

public interface IYogaClassService
{
    // Trainer: retrieve classes belonging to their own program.
    Task<IReadOnlyList<YogaClassResponse>> GetByProgramIdAsync(int trainerId,int yogaProgramId);

    // Customer: retrieve active classes from an available program.
    Task<IReadOnlyList<YogaClassResponse>> GetAvailableByProgramIdAsync(int yogaProgramId);

    // Trainer: retrieve a specific class they own.
    Task<YogaClassResponse?> GetByIdAsync(int trainerId, int id);

    // Customer: retrieve a specific active class.
    Task<YogaClassResponse?> GetAvailableByIdAsync(int id);

    // Trainer: create a class under their own program.
    Task<YogaClassResponse> CreateAsync(int trainerId,CreateYogaClassRequest request);

    // Trainer: update a class they own.
    Task<YogaClassResponse?> UpdateAsync(int trainerId,int id,UpdateYogaClassRequest request);

    // Trainer: delete a class they own.
    Task<bool> DeleteAsync(int trainerId, int id);
}
