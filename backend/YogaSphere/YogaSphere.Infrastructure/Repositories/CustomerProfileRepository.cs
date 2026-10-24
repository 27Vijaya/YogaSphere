
using Microsoft.EntityFrameworkCore;
using YogaSphere.Application.Interfaces.Repositories;
using YogaSphere.Domain.Entities;
using YogaSphere.Infrastructure.Persistence;

namespace YogaSphere.Infrastructure.Repositories;

public class CustomerProfileRepository : ICustomerProfileRepository
{
    private readonly YogaSphereDbContext _context;

    public CustomerProfileRepository(YogaSphereDbContext context)
    {
        _context = context;
    }

    // Find the profile associated with a specific user account.
    public async Task<CustomerProfile?> GetByUserIdAsync(int userId)
    {
        return await _context.CustomerProfiles
            .FirstOrDefaultAsync(profile => profile.UserId == userId);
    }

    // Add a new customer profile; SaveChangesAsync persists it separately.
    public async Task AddAsync(CustomerProfile profile)
    {
        await _context.CustomerProfiles.AddAsync(profile);
    }

    // Save pending changes to SQL Server.
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
