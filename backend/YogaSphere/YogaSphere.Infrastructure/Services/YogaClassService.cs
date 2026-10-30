using YogaSphere.Application.DTOs.YogaClass;
using YogaSphere.Application.Interfaces;
using YogaSphere.Application.Interfaces.Repositories;
using YogaSphere.Domain.Entities;

namespace YogaSphere.Infrastructure.Services;

public class YogaClassService : IYogaClassService
{
    private readonly IYogaClassRepository _yogaClassRepository;
    private readonly IYogaProgramRepository _yogaProgramRepository;
    private readonly ITrainerProfileRepository _trainerProfileRepository;

    public YogaClassService(
        IYogaClassRepository yogaClassRepository,
        IYogaProgramRepository yogaProgramRepository,
        ITrainerProfileRepository trainerProfileRepository)
    {
        _yogaClassRepository = yogaClassRepository;
        _yogaProgramRepository = yogaProgramRepository;
        _trainerProfileRepository = trainerProfileRepository;
    }

    // Retrieve classes belonging to a program owned by the trainer.
    public async Task<IReadOnlyList<YogaClassResponse>> GetByProgramIdAsync(
        int trainerId, int yogaProgramId)
    {
        var trainerProfile = await _trainerProfileRepository
            .GetByUserIdAsync(trainerId);

        if (trainerProfile is null)
            return Array.Empty<YogaClassResponse>();

        var program = await _yogaProgramRepository.GetByIdAsync(yogaProgramId);

        if (program is null || program.TrainerId != trainerProfile.Id)
            return Array.Empty<YogaClassResponse>();

        var classes = await _yogaClassRepository
            .GetByProgramIdAsync(yogaProgramId);

        return classes.Select(MapToResponse).ToList();
    }

    // Retrieve a class only if it belongs to the trainer's program.
    public async Task<YogaClassResponse?> GetByIdAsync(
        int trainerId, int id)
    {
        var trainerProfile = await _trainerProfileRepository
            .GetByUserIdAsync(trainerId);

        if (trainerProfile is null)
            return null;

        var yogaClass = await _yogaClassRepository.GetByIdAsync(id);

        if (yogaClass is null)
            return null;

        var program = await _yogaProgramRepository
            .GetByIdAsync(yogaClass.YogaProgramId);

        if (program is null || program.TrainerId != trainerProfile.Id)
            return null;

        return MapToResponse(yogaClass);
    }

    // Create a class under a program owned by the trainer.
    public async Task<YogaClassResponse> CreateAsync(
        int trainerId, CreateYogaClassRequest request)
    {
        var trainerProfile = await _trainerProfileRepository
            .GetByUserIdAsync(trainerId);

        if (trainerProfile is null)
            throw new InvalidOperationException("Trainer profile not found.");

        var program = await _yogaProgramRepository
            .GetByIdAsync(request.YogaProgramId);

        if (program is null || program.TrainerId != trainerProfile.Id)
            throw new InvalidOperationException(
                "Yoga program not found or does not belong to this trainer.");

        if (!program.IsActive)
            throw new InvalidOperationException(
                "Cannot add classes to an inactive yoga program.");

        var yogaClass = new YogaClass
        {
            YogaProgramId = request.YogaProgramId,
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            DurationMinutes = request.DurationMinutes,
            Capacity = request.Capacity,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _yogaClassRepository.AddAsync(yogaClass);
        await _yogaClassRepository.SaveChangesAsync();

        return MapToResponse(yogaClass);
    }

    // Update a class only if its program belongs to the trainer.
    public async Task<YogaClassResponse?> UpdateAsync(
        int trainerId, int id, UpdateYogaClassRequest request)
    {
        var existingClass = await GetOwnedClassAsync(trainerId, id);

        if (existingClass is null)
            return null;

        existingClass.Name = request.Name.Trim();
        existingClass.Description = request.Description?.Trim();
        existingClass.DurationMinutes = request.DurationMinutes;
        existingClass.Capacity = request.Capacity;
        existingClass.IsActive = request.IsActive;

        await _yogaClassRepository.SaveChangesAsync();

        return MapToResponse(existingClass);
    }

    // Delete a class only if its program belongs to the trainer.
    public async Task<bool> DeleteAsync(int trainerId, int id)
    {
        var existingClass = await GetOwnedClassAsync(trainerId, id);

        if (existingClass is null)
            return false;

        _yogaClassRepository.Delete(existingClass);
        await _yogaClassRepository.SaveChangesAsync();

        return true;
    }

    // Reuse the ownership check for update and delete.
    private async Task<YogaClass?> GetOwnedClassAsync(
        int trainerId, int id)
    {
        var trainerProfile = await _trainerProfileRepository
            .GetByUserIdAsync(trainerId);

        if (trainerProfile is null)
            return null;

        var yogaClass = await _yogaClassRepository.GetByIdAsync(id);

        if (yogaClass is null)
            return null;

        var program = await _yogaProgramRepository
            .GetByIdAsync(yogaClass.YogaProgramId);

        if (program is null || program.TrainerId != trainerProfile.Id)
            return null;

        return yogaClass;
    }

    // Map the database entity to its API response DTO.
    private static YogaClassResponse MapToResponse(YogaClass yogaClass)
    {
        return new YogaClassResponse
        {
            Id = yogaClass.Id,
            YogaProgramId = yogaClass.YogaProgramId,
            Name = yogaClass.Name,
            Description = yogaClass.Description,
            DurationMinutes = yogaClass.DurationMinutes,
            Capacity = yogaClass.Capacity,
            IsActive = yogaClass.IsActive,
            CreatedAt = yogaClass.CreatedAt
        };
    }
}