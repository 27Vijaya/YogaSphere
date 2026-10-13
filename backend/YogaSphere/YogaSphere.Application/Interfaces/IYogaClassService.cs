using System;
using System.Collections.Generic;
using System.Text;
using YogaSphere.Application.DTOs.YogaClass;

namespace YogaSphere.Application.Interfaces
{
    public interface IYogaClassService
    {
        //Retrives the available yoga classes for a given yoga program
        Task<IReadOnlyList<YogaClassResponse>> GetByProgramIdAsync(
       int yogaProgramId);

        //Retrives a yoga class by its id
        Task<YogaClassResponse?> GetByIdAsync(int id);

        //Creates a new yoga class for the given yoga program
        Task<YogaClassResponse> CreateAsync(
            CreateYogaClassRequest request);

        //Updates an existing yoga class for the given yoga program
        Task<YogaClassResponse> UpdateAsync(
            int id,
            UpdateYogaClassRequest request);

        //request deletion and reports wheather it succeeded
        Task<bool> DeleteAsync(int id);
    }
}
