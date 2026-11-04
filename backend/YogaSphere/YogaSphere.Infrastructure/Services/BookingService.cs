using YogaSphere.Application.DTOs.Booking;
using YogaSphere.Application.Interfaces;
using YogaSphere.Application.Interfaces.Repositories;
using YogaSphere.Domain.Entities;
using YogaSphere.Domain.Enums;

namespace YogaSphere.Infrastructure.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IYogaClassRepository _yogaClassRepository;
        private readonly ICustomerProfileRepository _customerProfileRepository;
        private readonly IYogaProgramRepository _yogaProgramRepository;

        public BookingService(
            IBookingRepository bookingRepository,
            IScheduleRepository scheduleRepository,
            IYogaClassRepository yogaClassRepository,
            ICustomerProfileRepository customerProfileRepository,
            IYogaProgramRepository yogaProgramRepository)
        {
            _bookingRepository = bookingRepository;
            _scheduleRepository = scheduleRepository;
            _yogaClassRepository = yogaClassRepository;
            _customerProfileRepository = customerProfileRepository;
            _yogaProgramRepository = yogaProgramRepository;
        }

        // Retrieves bookings belonging to the authenticated customer.
        public async Task<IReadOnlyList<BookingResponse>> GetMyBookingsAsync(
            int customerId)
        {
            var customer = await _customerProfileRepository
                .GetByUserIdAsync(customerId);

            if (customer == null)
                return Array.Empty<BookingResponse>();

            var bookings = await _bookingRepository
                .GetByCustomerIdAsync(customer.Id);

            return bookings.Select(MapToResponse).ToList();
        }

        // Retrieves a booking by its ID.
        public async Task<BookingResponse?> GetByIdAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);

            return booking == null ? null : MapToResponse(booking);
        }

        // Creates a booking after validating the customer and schedule.
        public async Task<BookingResponse> CreateAsync(
            int customerId,
            CreateBookingRequest request)
        {
            // Resolve the customer profile using the JWT user ID.
            var customer = await _customerProfileRepository
                .GetByUserIdAsync(customerId);

            if (customer == null)
                throw new InvalidOperationException(
                    "Customer profile was not found.");

            // Retrieve the requested schedule.
            var schedule = await _scheduleRepository
                .GetByIdAsync(request.ScheduleId);

            if (schedule == null || !schedule.IsActive)
                throw new InvalidOperationException(
                    "Schedule is unavailable.");

            if (schedule.StartTime <= DateTime.UtcNow)
                throw new InvalidOperationException(
                    "Cannot book a schedule that has already started.");

            // Check whether the associated yoga class is active.
            var yogaClass = await _yogaClassRepository
                .GetByIdAsync(schedule.YogaClassId);

            if (yogaClass == null || !yogaClass.IsActive)
                throw new InvalidOperationException(
                    "Yoga class is unavailable.");

            // Ensure the yoga program associated with the class is active.
            var yogaProgram = await _yogaProgramRepository
                .GetByIdAsync(yogaClass.YogaProgramId);

            if (yogaProgram == null || !yogaProgram.IsActive)
                throw new InvalidOperationException(
                    "The yoga program is unavailable.");

            // Prevent duplicate active bookings.
            var alreadyBooked = await _bookingRepository
                .HasExistingBookingAsync(customer.Id, schedule.Id);

            if (alreadyBooked)
                throw new InvalidOperationException(
                    "You already have an active booking for this schedule.");

            // Check the class capacity.
            var activeBookingCount = await _bookingRepository
                .GetActiveBookingCountAsync(schedule.Id);

            if (activeBookingCount >= yogaClass.Capacity)
                throw new InvalidOperationException(
                    "This schedule is fully booked.");

            var now = DateTime.UtcNow;

            var booking = new Booking
            {
                CustomerId = customer.Id,
                ScheduleId = schedule.Id,
                Status = BookingStatus.Confirmed,
                BookingDate = now,
                CreatedAt = now
            };

            await _bookingRepository.AddAsync(booking);
            await _bookingRepository.SaveChangesAsync();

            return MapToResponse(booking);
        }

        // Cancels a booking belonging to the specified customer.
        public async Task<bool> CancelAsync(int id, int customerId)
        {
            var customer = await _customerProfileRepository
                .GetByUserIdAsync(customerId);

            if (customer == null)
                return false;

            var booking = await _bookingRepository.GetByIdAsync(id);

            if (booking == null || booking.CustomerId != customer.Id)
                return false;

            if (booking.Status != BookingStatus.Pending &&
                booking.Status != BookingStatus.Confirmed)
            {
                throw new InvalidOperationException(
                    "This booking cannot be cancelled.");
            }

            booking.Status = BookingStatus.Cancelled;
            booking.CancelledAt = DateTime.UtcNow;

            await _bookingRepository.SaveChangesAsync();

            return true;
        }

        // Maps the database entity to the API response DTO.
        private static BookingResponse MapToResponse(Booking booking)
        {
            return new BookingResponse
            {
                Id = booking.Id,
                CustomerId = booking.CustomerId,
                ScheduleId = booking.ScheduleId,
                Status = booking.Status,
                BookingDate = booking.BookingDate,
                CancelledAt = booking.CancelledAt,
                CreatedAt = booking.CreatedAt
            };
        }
    }
}