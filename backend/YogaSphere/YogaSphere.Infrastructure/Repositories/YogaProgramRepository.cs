
using Microsoft.EntityFrameworkCore;
using YogaSphere.Application.Interfaces.Repositories;
using YogaSphere.Domain.Entities;
using YogaSphere.Infrastructure.Persistence;

namespace YogaSphere.Infrastructure.Repositories;

public class YogaProgramRepository : IYogaProgramRepository
{
    private readonly YogaSphereDbContext _context;

    public YogaProgramRepository(YogaSphereDbContext context)
    {
        _context = context;
    }

    // Retrieve all yoga programs.
    public async Task<IReadOnlyList<YogaProgram>> GetAllAsync()
    {
        return await _context.YogaPrograms
            .AsNoTracking()
            .ToListAsync();
    }

    // Retrieve one yoga program by its ID.
    public async Task<YogaProgram?> GetByIdAsync(int id)
    {
        return await _context.YogaPrograms
            .FirstOrDefaultAsync(program => program.Id == id);
    }

    // Add a new yoga program; saving happens separately.
    public async Task AddAsync(YogaProgram program)
    {
        await _context.YogaPrograms.AddAsync(program);
    }

    // Persist pending changes to SQL Server.
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    // mark the entity for deletion.
    public void Delete(YogaProgram program)
    {
        _context.YogaPrograms.Remove(program);
    }
}
