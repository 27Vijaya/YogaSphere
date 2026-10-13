using System;
using System.Collections.Generic;
using System.Text;
using YogaSphere.Domain.Entities;

namespace YogaSphere.Application.Interfaces.Repositories
{
    public interface IBookingRepository
    {
        Task<IReadOnlyList<Booking>> GetByCustomerIdAsync(
            int customerId);

        Task<Booking?> GetByIdAsync(int id);

        //checks whether the customer already has a booking for the schedule
        Task<bool> HasExistingBookingAsync(
            int customerId,
            int scheduleId);

        //counts bookings that should occupy a session's capacity
        Task<int> GetActiveBookingCountAsync(int scheduleId);

        Task AddAsync(Booking booking);

        Task SaveChangesAsync();
    }
}
