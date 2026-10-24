
using Microsoft.EntityFrameworkCore;
using YogaSphere.Application.Interfaces.Repositories;
using YogaSphere.Domain.Entities;
using YogaSphere.Infrastructure.Persistence;

namespace YogaSphere.Infrastructure.Repositories;

public class TrainerProfileRepository : ITrainerProfileRepository
{
    private readonly YogaSphereDbContext _context;

    public TrainerProfileRepository(YogaSphereDbContext context)
    {
        _context = context;
    }

    // Find the trainer profile associated with a user account.
    public async Task<TrainerProfile?> GetByUserIdAsync(int userId)
    {
        return await _context.TrainerProfiles
            .FirstOrDefaultAsync(profile => profile.UserId == userId);
    }

    // Add a new trainer profile; changes are saved separately.
    public async Task AddAsync(TrainerProfile profile)
    {
        await _context.TrainerProfiles.AddAsync(profile);
    }

    // Persist pending changes to SQL Server.
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
