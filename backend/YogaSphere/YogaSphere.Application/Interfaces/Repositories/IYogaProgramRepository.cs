using System;
using System.Collections.Generic;
using System.Text;
using YogaSphere.Domain.Entities;

namespace YogaSphere.Application.Interfaces.Repositories
{
    public interface IYogaProgramRepository
    {

        Task<IReadOnlyList<YogaProgram>> GetAllAsync();

        Task<YogaProgram?> GetByIdAsync(int id);

        Task AddAsync(YogaProgram program);

        Task SaveChangesAsync();

        void Delete(YogaProgram program);
    }
}
