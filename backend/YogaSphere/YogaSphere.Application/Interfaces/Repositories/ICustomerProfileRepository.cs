using System;
using System.Collections.Generic;
using System.Text;
using YogaSphere.Domain.Entities;

namespace YogaSphere.Application.Interfaces.Repositories
{
    //defines how the service requests customer profile data to be retrieved or saved
    public interface ICustomerProfileRepository
    {
        //retrieves the profile associated with a user
        Task<CustomerProfile?> GetByUserIdAsync(int userId);

        //adds a new customer profile to the database
        Task AddAsync(CustomerProfile profile);

        //saves changes made to the customer profile in the database
        Task SaveChangesAsync();
    }
}
