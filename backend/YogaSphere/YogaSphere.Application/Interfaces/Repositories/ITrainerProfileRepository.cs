using System;
using System.Collections.Generic;
using System.Text;
using YogaSphere.Domain.Entities;

namespace YogaSphere.Application.Interfaces.Repositories
{
    //stores trainer specific info seperately from general user
    public interface ITrainerProfileRepository
    {
        //retrieves the profile associated with a user
        Task<TrainerProfile?> GetByUserIdAsync(int userId);

        //adds a new trainer profile to the database
        Task AddAsync(TrainerProfile profile);

        //persists pending changes
        Task SaveChangesAsync();
    }
}
