using System;
using System.Collections.Generic;
using System.Text;
using YogaSphere.Domain.Entities;

namespace YogaSphere.Application.Interfaces.Repositories
{
    public interface IReviewRepository
    {
        Task<IReadOnlyList<Review>> GetByYogaClassIdAsync(
            int yogaClassId);

        Task<Review?> GetByIdAsync(int reviewId);

        Task AddAsync(Review review);

        Task DeleteAsync(Review review);

        Task SaveChangesAsync();
    }
}
