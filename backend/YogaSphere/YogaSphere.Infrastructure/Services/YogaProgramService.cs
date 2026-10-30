
using YogaSphere.Application.DTOs.YogaProgram;
using YogaSphere.Application.Interfaces;
using YogaSphere.Application.Interfaces.Repositories;
using YogaSphere.Domain.Entities;

namespace YogaSphere.Infrastructure.Services;

public class YogaProgramService : IYogaProgramService
{
    private readonly IYogaProgramRepository _yogaProgramRepository;

    private readonly ITrainerProfileRepository _trainerProfileRepository;

    public YogaProgramService(IYogaProgramRepository yogaProgramRepository,
        ITrainerProfileRepository trainerProfileRepository)
    {
        _yogaProgramRepository = yogaProgramRepository;
        _trainerProfileRepository = trainerProfileRepository;
    }

    // Retrieve all programs and map them to response DTOs.
    public async Task<IReadOnlyList<YogaProgramResponse>> GetAllAsync()
    {
        var programs = await _yogaProgramRepository.GetAllAsync();

        return programs.Select(MapToResponse).ToList();
    }

    // Retrieve one program by its ID.
    public async Task<YogaProgramResponse?> GetByIdAsync(int id)
    {
        var program = await _yogaProgramRepository.GetByIdAsync(id);

        return program is null ? null : MapToResponse(program);
    }

    // Create a program owned by the authenticated trainer.
    public async Task<YogaProgramResponse> CreateAsync(
    int trainerId, CreateYogaProgramRequest request)
    {
        // Find the trainer profile using the authenticated user's ID.
        var trainerProfile = await _trainerProfileRepository
            .GetByUserIdAsync(trainerId);

        // A trainer must have a profile before creating programs.
        if (trainerProfile is null)
        {
            throw new InvalidOperationException(
                "Trainer profile not found.");
        }

        var program = new YogaProgram
        {
            // Store the trainer profile ID, not the user ID.
            TrainerId = trainerProfile.Id,
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            DurationWeeks = request.DurationWeeks,
            Price = request.Price,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _yogaProgramRepository.AddAsync(program);
        await _yogaProgramRepository.SaveChangesAsync();

        return MapToResponse(program);
    }

    // Update a program only when it belongs to this trainer.
    public async Task<YogaProgramResponse?> UpdateAsync(
    int trainerId, int programId, UpdateYogaProgramRequest request)
    {
        // Find the trainer profile using the authenticated user's ID.
        var trainerProfile = await _trainerProfileRepository
            .GetByUserIdAsync(trainerId);

        if (trainerProfile is null)
        {
            return null;
        }

        var program = await _yogaProgramRepository.GetByIdAsync(programId);

        // Ensure the program belongs to this trainer profile.
        if (program is null || program.TrainerId != trainerProfile.Id)
        {
            return null;
        }

        program.Name = request.Name.Trim();
        program.Description = request.Description?.Trim();
        program.DurationWeeks = request.DurationWeeks;
        program.Price = request.Price;
        program.IsActive = request.IsActive;

        await _yogaProgramRepository.SaveChangesAsync();

        return MapToResponse(program);
    }


    // Delete a program only when it belongs to this trainer.
    public async Task<bool> DeleteAsync(int trainerId, int programId)
    {
        // Find the trainer profile using the authenticated user's ID.
        var trainerProfile = await _trainerProfileRepository
            .GetByUserIdAsync(trainerId);

        if (trainerProfile is null)
        {
            return false;
        }

        var program = await _yogaProgramRepository.GetByIdAsync(programId);

        // Only allow the owner of the program to delete it.
        if (program is null || program.TrainerId != trainerProfile.Id)
        {
            return false;
        }

        _yogaProgramRepository.Delete(program);
        await _yogaProgramRepository.SaveChangesAsync();

        return true;
    }


    // Convert a domain entity to the API response DTO.
    private static YogaProgramResponse MapToResponse(YogaProgram program)
    {
        return new YogaProgramResponse
        {
            Id = program.Id,
            TrainerId = program.TrainerId,
            Name = program.Name,
            Description = program.Description,
            DurationWeeks = program.DurationWeeks,
            Price = program.Price,
            IsActive = program.IsActive,
            CreatedAt = program.CreatedAt
        };
    }
}
