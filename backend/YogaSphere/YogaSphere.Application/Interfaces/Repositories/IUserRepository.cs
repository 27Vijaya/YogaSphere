
using YogaSphere.Domain.Entities;

namespace YogaSphere.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int userId);

    Task<User?> GetByEmailAsync(string email);

    Task<List<User>> GetAllAsync();

    Task AddAsync(User user);

    void Update(User user);

    void Delete(User user);

    Task SaveChangesAsync();
}
