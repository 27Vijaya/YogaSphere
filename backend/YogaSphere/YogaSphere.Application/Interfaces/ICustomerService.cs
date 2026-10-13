using System;
using System.Collections.Generic;
using System.Text;
using YogaSphere.Application.DTOs.Customer;

namespace YogaSphere.Application.Interfaces
{
    public interface ICustomerService
    {
        //Accepts userId and returns customer profile details
        Task<CustomerProfileResponse?> GetProfileAsync(int userId);

        //Accepts userId and updated profile details
        Task<CustomerProfileResponse> UpdateProfileAsync(
            int userId, UpdateCustomerProfileRequest request);
    }
}
