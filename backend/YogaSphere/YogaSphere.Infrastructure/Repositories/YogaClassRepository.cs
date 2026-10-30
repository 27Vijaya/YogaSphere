using Microsoft.EntityFrameworkCore;
using YogaSphere.Application.Interfaces.Repositories;
using YogaSphere.Domain.Entities;
using YogaSphere.Infrastructure.Persistence;

namespace YogaSphere.Infrastructure.Repositories;

public class YogaClassRepository : IYogaClassRepository
{
    private readonly YogaSphereDbContext _context;

    public YogaClassRepository(YogaSphereDbContext context)
    {
        _context = context;
    }

    // Retrieve all classes belonging to a yoga program.
    public async Task<IReadOnlyList<YogaClass>> GetByProgramIdAsync(
        int yogaProgramId)
    {
        return await _context.YogaClasses
            .AsNoTracking()
            .Where(yogaClass => yogaClass.YogaProgramId == yogaProgramId)
            .ToListAsync();
    }

    // Retrieve a yoga class by its ID.
    public async Task<YogaClass?> GetByIdAsync(int id)
    {
        return await _context.YogaClasses
            .FirstOrDefaultAsync(yogaClass => yogaClass.Id == id);
    }

    // Add a new yoga class.
    public async Task AddAsync(YogaClass yogaClass)
    {
        await _context.YogaClasses.AddAsync(yogaClass);
    }

    // Mark a yoga class for deletion.
    public void Delete(YogaClass yogaClass)
    {
        _context.YogaClasses.Remove(yogaClass);
    }

    // Persist pending changes to SQL Server.
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}