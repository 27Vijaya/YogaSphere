
using YogaSphere.Application.DTOs.Customer;
using YogaSphere.Application.Interfaces;
using YogaSphere.Application.Interfaces.Repositories;
using YogaSphere.Domain.Entities;

namespace YogaSphere.Infrastructure.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerProfileRepository _customerProfileRepository;

    public CustomerService(
        ICustomerProfileRepository customerProfileRepository)
    {
        _customerProfileRepository = customerProfileRepository;
    }

    // Retrieve the customer profile associated with a user account.
    public async Task<CustomerProfileResponse?> GetProfileAsync(int userId)
    {
        var profile = await _customerProfileRepository
            .GetByUserIdAsync(userId);

        if (profile is null)
        {
            return null;
        }

        return MapToResponse(profile);
    }

    // Update an existing profile or create one if it does not exist.
    public async Task<CustomerProfileResponse> UpdateProfileAsync(
        int userId,
        UpdateCustomerProfileRequest request)
    {
        var profile = await _customerProfileRepository
            .GetByUserIdAsync(userId);

        if (profile is null)
        {
            profile = new CustomerProfile
            {
                UserId = userId
            };

            await _customerProfileRepository.AddAsync(profile);
        }

        profile.PhoneNumber = request.PhoneNumber;
        profile.DateOfBirth = request.DateOfBirth;
        profile.Address = request.Address;
        profile.EmergencyContact = request.EmergencyContact;

        await _customerProfileRepository.SaveChangesAsync();

        return MapToResponse(profile);
    }

    // Keep response mapping in one place to avoid repeating code.
    private static CustomerProfileResponse MapToResponse(
        CustomerProfile profile)
    {
        return new CustomerProfileResponse
        {
            Id = profile.Id,
            UserId = profile.UserId,
            PhoneNumber = profile.PhoneNumber,
            DateOfBirth = profile.DateOfBirth,
            Address = profile.Address,
            EmergencyContact = profile.EmergencyContact
        };
    }
}
