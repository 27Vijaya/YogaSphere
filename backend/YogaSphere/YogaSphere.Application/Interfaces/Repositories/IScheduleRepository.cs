using System;
using System.Collections.Generic;
using System.Text;
using YogaSphere.Domain.Entities;

namespace YogaSphere.Application.Interfaces.Repositories
{
    public interface IScheduleRepository
    {
        //retrieves schedules for a particular class
        Task<IReadOnlyList<Schedule>> GetByYogaClassIdAsync(
        int yogaClassId);

        Task<Schedule?> GetByIdAsync(int id);

        Task AddAsync(Schedule schedule);

        //checks whether a proposed time overlaps an existing schedule for the same class
        Task<bool> HasConflictAsync(
            int yogaClassId,
            DateTime startTime,
            DateTime endTime,
            int? excludeScheduleId = null);

        Task SaveChangesAsync();

        void Delete(Schedule schedule);
    }
}
