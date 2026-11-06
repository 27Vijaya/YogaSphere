using YogaSphere.Application.DTOs.Payment;
using YogaSphere.Application.Interfaces;
using YogaSphere.Application.Interfaces.Repositories;
using YogaSphere.Domain.Entities;
using YogaSphere.Domain.Enums;

namespace YogaSphere.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ICustomerProfileRepository _customerProfileRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IYogaClassRepository _yogaClassRepository;
        private readonly IYogaProgramRepository _yogaProgramRepository;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IBookingRepository bookingRepository,
            ICustomerProfileRepository customerProfileRepository,
            IScheduleRepository scheduleRepository,
            IYogaClassRepository yogaClassRepository,
            IYogaProgramRepository yogaProgramRepository)
        {
            _paymentRepository = paymentRepository;
            _bookingRepository = bookingRepository;
            _customerProfileRepository = customerProfileRepository;
            _scheduleRepository = scheduleRepository;
            _yogaClassRepository = yogaClassRepository;
            _yogaProgramRepository = yogaProgramRepository;
        }

        // Creates a pending payment for a booking owned by the customer.
        public async Task<PaymentInitiationResponse> InitiateAsync(
            int customerId,
            CreatePaymentRequest request)
        {
            // Validate the selected payment method.
            if (!Enum.IsDefined(typeof(PaymentMethod), request.PaymentMethod))
                throw new InvalidOperationException("Invalid payment method.");

            var customer = await _customerProfileRepository
                .GetByUserIdAsync(customerId);

            if (customer == null)
                throw new InvalidOperationException("Customer profile not found.");

            var booking = await _bookingRepository
                .GetByIdAsync(request.BookingId);

            if (booking == null || booking.CustomerId != customer.Id)
                throw new InvalidOperationException("Booking not found.");

            if (booking.Status != BookingStatus.Pending)
                throw new InvalidOperationException(
                    "Payment can only be initiated for pending bookings.");

            // Prevent another pending or successful payment attempt.
            var existingPayments = await _paymentRepository
                .GetByBookingIdAsync(booking.Id);

            if (existingPayments.Any(payment =>
                payment.PaymentStatus == PaymentStatus.Pending ||
                payment.PaymentStatus == PaymentStatus.Successful))
            {
                throw new InvalidOperationException(
                    "A pending or successful payment already exists for this booking.");
            }

            // Find the program price through the booking's schedule and class.
            var schedule = await _scheduleRepository
                .GetByIdAsync(booking.ScheduleId);

            if (schedule == null)
                throw new InvalidOperationException("Schedule not found.");

            var yogaClass = await _yogaClassRepository
                .GetByIdAsync(schedule.YogaClassId);

            if (yogaClass == null)
                throw new InvalidOperationException("Yoga class not found.");

            var program = await _yogaProgramRepository
                .GetByIdAsync(yogaClass.YogaProgramId);

            if (program == null)
                throw new InvalidOperationException("Yoga program not found.");

            // The amount is calculated on the server, never accepted from the client.
            var payment = new Payment
            {
                BookingId = booking.Id,
                Amount = program.Price,
                PaymentMethod = request.PaymentMethod,
                PaymentStatus = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            // This is a mock order ID, not a real payment gateway order.
            return new PaymentInitiationResponse
            {
                PaymentId = payment.Id,
                BookingId = booking.Id,
                Amount = payment.Amount,
                Currency = "INR",
                GatewayOrderId = $"MOCK-{Guid.NewGuid():N}",
                GatewayName = "MockGateway"
            };
        }

        // Retrieves a payment only when it belongs to the authenticated customer.
        public async Task<PaymentResponse?> GetByIdAsync(
            int customerId,
            int paymentId)
        {
            var customer = await _customerProfileRepository
                .GetByUserIdAsync(customerId);

            if (customer == null)
                return null;

            var payment = await _paymentRepository.GetByIdAsync(paymentId);

            if (payment == null || payment.BookingId <= 0)
                return null;

            var booking = await _bookingRepository
                .GetByIdAsync(payment.BookingId);

            if (booking == null || booking.CustomerId != customer.Id)
                return null;

            return MapToResponse(payment);
        }

        // Retrieves payment history after verifying booking ownership.
        public async Task<IReadOnlyList<PaymentResponse>> GetByBookingIdAsync(
            int customerId,
            int bookingId)
        {
            var customer = await _customerProfileRepository
                .GetByUserIdAsync(customerId);

            if (customer == null)
                return Array.Empty<PaymentResponse>();

            var booking = await _bookingRepository.GetByIdAsync(bookingId);

            if (booking == null || booking.CustomerId != customer.Id)
                return Array.Empty<PaymentResponse>();

            var payments = await _paymentRepository
                .GetByBookingIdAsync(bookingId);

            return payments.Select(MapToResponse).ToList();
        }

        // Maps the Payment entity to its response DTO.
        private static PaymentResponse MapToResponse(Payment payment)
        {
            return new PaymentResponse
            {
                Id = payment.Id,
                BookingId = payment.BookingId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                PaymentStatus = payment.PaymentStatus,
                TransactionId = payment.TransactionId,
                PaidAt = payment.PaidAt,
                CreatedAt = payment.CreatedAt
            };
        }
    }
}