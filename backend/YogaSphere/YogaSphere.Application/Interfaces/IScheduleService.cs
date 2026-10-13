using System;
using System.Collections.Generic;
using System.Text;
using YogaSphere.Application.DTOs.Schedule;

namespace YogaSphere.Application.Interfaces
{
    internal interface IScheduleService
    {
        //Retrives the available schedules for a given yoga class
        Task<IReadOnlyList<ScheduleResponse>> GetByYogaClassIdAsync(
        int yogaClassId);

        //Retrives a schedule by its id
        Task<ScheduleResponse?> GetByIdAsync(int id);

        //Creates a new schedule for the given yoga class
        Task<ScheduleResponse> CreateAsync(
            CreateScheduleRequest request);

        //Updates an existing schedule for the given yoga class
        Task<ScheduleResponse> UpdateAsync(
            int id,
            UpdateScheduleRequest request);

        //request deletion and reports wheather it succeeded
        Task<bool> DeleteAsync(int id);
    }
}
