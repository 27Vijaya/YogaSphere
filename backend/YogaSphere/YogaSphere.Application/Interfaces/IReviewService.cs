using System;
using System.Collections.Generic;
using System.Text;
using YogaSphere.Application.DTOs.Review;

namespace YogaSphere.Application.Interfaces
{
    public interface IReviewService
    {
        //retrieves reviews for a particular yoga class
        Task<IReadOnlyList<ReviewResponse>> GetByYogaClassIdAsync(
        int yogaClassId);

        //submits a review on behalf of a customer
        Task<ReviewResponse> CreateAsync(
            int customerId,
            CreateReviewRequest request);

        //allows a customer to request deletion of their own review
        Task<bool> DeleteAsync(int reviewId, int customerId);
    }
}
