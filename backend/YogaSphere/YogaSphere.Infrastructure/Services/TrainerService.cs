
using YogaSphere.Application.DTOs.Trainer;
using YogaSphere.Application.Interfaces;
using YogaSphere.Application.Interfaces.Repositories;
using YogaSphere.Domain.Entities;

namespace YogaSphere.Infrastructure.Services;

public class TrainerService : ITrainerService
{
    private readonly ITrainerProfileRepository _trainerProfileRepository;

    public TrainerService(
        ITrainerProfileRepository trainerProfileRepository)
    {
        _trainerProfileRepository = trainerProfileRepository;
    }

    // Retrieve the trainer profile associated with a user account.
    public async Task<TrainerProfileResponse?> GetProfileAsync(int userId)
    {
        var profile = await _trainerProfileRepository
            .GetByUserIdAsync(userId);

        if (profile is null)
        {
            return null;
        }

        return MapToResponse(profile);
    }

    // Update an existing profile or create one if it doesn't exist.
    public async Task<TrainerProfileResponse> UpdateProfileAsync(
        int userId,
        UpdateTrainerProfileRequest request)
    {
        var profile = await _trainerProfileRepository
            .GetByUserIdAsync(userId);

        if (profile is null)
        {
            profile = new TrainerProfile
            {
                UserId = userId
            };

            await _trainerProfileRepository.AddAsync(profile);
        }

        // Map DTO property names to the existing Domain entity properties.
        profile.Spacialization = request.Specialization;
        profile.Experience = request.ExperienceYears;
        profile.Certifications = request.Certification;
        profile.Bio = request.Bio;

        await _trainerProfileRepository.SaveChangesAsync();

        return MapToResponse(profile);
    }

    // Convert the Domain entity into the API response DTO.
    private static TrainerProfileResponse MapToResponse(
        TrainerProfile profile)
    {
        return new TrainerProfileResponse
        {
            Id = profile.Id,
            UserId = profile.UserId,
            Specialization = profile.Spacialization,
            ExperienceYears = profile.Experience,
            Certification = profile.Certifications,
            Bio = profile.Bio
        };
    }
}
