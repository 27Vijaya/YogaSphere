using System;
using System.Collections.Generic;
using System.Text;
using YogaSphere.Domain.Entities;

namespace YogaSphere.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        //retrieves a user when needed
        Task<User?> GetByIdAsync(int userId);

        //lets the authentication service check whether an email already exists and find a user during login
        Task<User?> GetByEmailAsync(string email);

        Task AddAsync(User user);

        Task SaveChangesAsync();
    }
}
