using Microsoft.EntityFrameworkCore;
using YogaSphere.Application.Interfaces.Repositories;
using YogaSphere.Domain.Entities;
using YogaSphere.Domain.Enums;
using YogaSphere.Infrastructure.Persistence;

namespace YogaSphere.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly YogaSphereDbContext _context;

        public BookingRepository(YogaSphereDbContext context)
        {
            _context = context;
        }

        // Retrieves all bookings belonging to a customer.
        public async Task<IReadOnlyList<Booking>> GetByCustomerIdAsync(
            int customerId)
        {
            return await _context.Bookings
                .AsNoTracking()
                .Where(booking => booking.CustomerId == customerId)
                .OrderByDescending(booking => booking.CreatedAt)
                .ToListAsync();
        }

        // Retrieves a booking using its ID.
        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .FirstOrDefaultAsync(booking => booking.Id == id);
        }

        // Checks whether the customer already has an active booking.
        public async Task<bool> HasExistingBookingAsync(
            int customerId,
            int scheduleId)
        {
            return await _context.Bookings.AnyAsync(booking =>
                booking.CustomerId == customerId &&
                booking.ScheduleId == scheduleId &&
                (booking.Status == BookingStatus.Pending ||
                 booking.Status == BookingStatus.Confirmed));
        }

        // Counts bookings that occupy the schedule's capacity.
        public async Task<int> GetActiveBookingCountAsync(int scheduleId)
        {
            return await _context.Bookings.CountAsync(booking =>
                booking.ScheduleId == scheduleId &&
                (booking.Status == BookingStatus.Pending ||
                 booking.Status == BookingStatus.Confirmed));
        }

        // Adds a new booking to the database context.
        public async Task AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
        }

        // Persists changes to the database.
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}