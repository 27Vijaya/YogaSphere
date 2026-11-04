using System;
using System.Collections.Generic;
using System.Text;
using YogaSphere.Application.DTOs.Booking;
using YogaSphere.Domain.Enums;

namespace YogaSphere.Application.Interfaces
{
    public interface IBookingService
    {
        //retrieves a customer's bookings
        Task<IReadOnlyList<BookingResponse>> GetMyBookingsAsync(
        int customerId);

        //retrieves a specific booking
        Task<BookingResponse?> GetByIdAsync(int id);

        //books a scheduled session
        Task<BookingResponse> CreateAsync(
            int customerId,
            CreateBookingRequest request);

        //cancels a booking for the specified customer
        Task<bool> CancelAsync(int id, int customerId);
    }
}
