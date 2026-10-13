using System;
using System.Collections.Generic;
using System.Text;
using YogaSphere.Application.DTOs.Trainer;

namespace YogaSphere.Application.Interfaces
{
    public interface ITrainerService
    {
        //retrives a trainer's profile
        //? - if Trainer profile not exists for the given userId, returns null
        Task<TrainerProfileResponse?> GetProfileAsync(int userId);

        //updates specialization, experience, certifications, and biography
        Task<TrainerProfileResponse> UpdateProfileAsync(
            int userId, UpdateTrainerProfileRequest request);
    }
}
