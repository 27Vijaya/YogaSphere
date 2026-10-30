using System;
using System.Collections.Generic;
using System.Text;
using YogaSphere.Domain.Entities;

namespace YogaSphere.Application.Interfaces.Repositories
{
    public interface IYogaClassRepository
    {
        Task<IReadOnlyList<YogaClass>> GetByProgramIdAsync(
            int yogaProgramId);

        Task<YogaClass?> GetByIdAsync(int id);

        Task AddAsync(YogaClass yogaClass);

        Task SaveChangesAsync();

        void Delete(YogaClass yogaClass);
    }
}
