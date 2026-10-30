using System;
using System.Collections.Generic;
using System.Text;
using YogaSphere.Application.DTOs.YogaProgram;

namespace YogaSphere.Application.Interfaces
{
    public interface IYogaProgramService
    {
        //Retrives the available yoga programs
        Task<IReadOnlyList<YogaProgramResponse>> GetAllAsync();

        //Retrives a yoga program by its id
        Task<YogaProgramResponse?> GetByIdAsync(int id);

        //Creates a new yoga program for the given trainer
        Task<YogaProgramResponse> CreateAsync(
            int trainerId, CreateYogaProgramRequest request);

        // Returns null if the program doesn't exist or belongs to another trainer
        Task<YogaProgramResponse?> UpdateAsync(
            int trainerId, int programId, UpdateYogaProgramRequest request);

        // Delete a program only if it belongs to the specified trainer.
        Task<bool> DeleteAsync(int trainerId, int programId);
    }
}
