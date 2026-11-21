using YogaSphere.Application.DTOs.Payment;
using YogaSphere.Application.Interfaces;
using YogaSphere.Application.Interfaces.Repositories;
using YogaSphere.Domain.Entities;
using YogaSphere.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using YogaSphere.Infrastructure.Persistence;

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
        private readonly IRazorpayService _razorpayService;
        private readonly YogaSphereDbContext _context;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IBookingRepository bookingRepository,
            ICustomerProfileRepository customerProfileRepository,
            IScheduleRepository scheduleRepository,
            IYogaClassRepository yogaClassRepository,
            IYogaProgramRepository yogaProgramRepository,
            IRazorpayService razorpayService,
            YogaSphereDbContext context)
        {
            _paymentRepository = paymentRepository;
            _bookingRepository = bookingRepository;
            _customerProfileRepository = customerProfileRepository;
            _scheduleRepository = scheduleRepository;
            _yogaClassRepository = yogaClassRepository;
            _yogaProgramRepository = yogaProgramRepository;
            _razorpayService = razorpayService;
            _context = context;
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

            // Allow only one retry after the initial failed payment.
            var failedAttempts = existingPayments.Count(payment =>
                payment.PaymentStatus == PaymentStatus.Failed);

            if (failedAttempts >= 2)
            {
                throw new InvalidOperationException(
                    "Payment retry limit reached for this booking.");
            }

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

            var razorpayOrder = await _razorpayService.CreateOrderAsync(program.Price,"INR");

            // The amount is calculated on the server, never accepted from the client.
            var payment = new Payment
            {
                BookingId = booking.Id,
                Amount = program.Price,
                PaymentMethod = request.PaymentMethod,
                PaymentStatus = PaymentStatus.Pending,
                GatewayOrderId = razorpayOrder.OrderId,
                CreatedAt = DateTime.UtcNow
            };

            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            // Return the Razorpay order details required by the frontend Checkout.
            return new PaymentInitiationResponse
            {
                PaymentId = payment.Id,
                BookingId = booking.Id,
                Amount = payment.Amount,
                Currency = "INR",
                GatewayOrderId = razorpayOrder.OrderId,
                GatewayName = "Razorpay",
                KeyId = razorpayOrder.KeyId
            };
        }

        // Verifies Razorpay payment and confirms the booking only after successful verification.
        public async Task<PaymentResponse?> VerifyPaymentAsync(
            int customerId,
            VerifyPaymentRequest request)
        {
            // Find the customer profile.
            var customer = await _customerProfileRepository
                .GetByUserIdAsync(customerId);

            if (customer == null)
                return null;

            // Find the payment record.
            var payment = await _paymentRepository
                .GetByIdAsync(request.PaymentId);

            if (payment == null)
                return null;

            // Verify booking ownership.
            var booking = await _bookingRepository
                .GetByIdAsync(payment.BookingId);

            if (booking == null || booking.CustomerId != customer.Id)
                return null;

            // Return the existing result if this exact payment was already verified.
            if (payment.PaymentStatus == PaymentStatus.Successful &&
                payment.TransactionId == request.RazorpayPaymentId)
            {
                return MapToResponse(payment);
            }

            // Verify the Razorpay order ID matches our stored order.
            if (payment.GatewayOrderId != request.RazorpayOrderId)
                throw new InvalidOperationException("Invalid Razorpay order ID.");

            // Prevent processing an already completed payment.
            if (payment.PaymentStatus != PaymentStatus.Pending ||
                booking.Status != BookingStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Payment or booking is not in a pending state.");
            }

            // Verify Razorpay's cryptographic signature.
            var isValid = _razorpayService.VerifyPaymentSignature(
                request.RazorpayOrderId,
                request.RazorpayPaymentId,
                request.RazorpaySignature);

            if (!isValid)
                throw new InvalidOperationException(
                    "Razorpay payment signature verification failed.");

            // Retrieve the actual payment details directly from Razorpay.
            var razorpayPayment = await _razorpayService.GetPaymentDetailsAsync(
                request.RazorpayPaymentId);

            // Verify that the payment belongs to our stored order.
            if (razorpayPayment.OrderId != payment.GatewayOrderId)
            {
                throw new InvalidOperationException(
                    "Razorpay payment order ID mismatch.");
            }

            // Convert the expected amount from rupees to paise.
            var expectedAmountInPaise = (long)Math.Round(
                payment.Amount * 100,
                MidpointRounding.AwayFromZero);

            // Verify the payment amount and currency.
            if (razorpayPayment.Amount != expectedAmountInPaise ||
                razorpayPayment.Currency != "INR")
            {
                throw new InvalidOperationException(
                    "Razorpay payment amount or currency mismatch.");
            }

            // Handle an explicitly failed payment reported by Razorpay.
            if (razorpayPayment.Status == "failed")
            {
                payment.PaymentStatus = PaymentStatus.Failed;
                payment.TransactionId = request.RazorpayPaymentId;

                await _paymentRepository.SaveChangesAsync();

                throw new InvalidOperationException(
                    "Razorpay payment failed. You can retry payment once.");
            }

            // Confirm that Razorpay has actually captured the payment.
            if (razorpayPayment.Status != "captured" ||
                !razorpayPayment.Captured)
            {
                throw new InvalidOperationException(
                    "Razorpay payment has not been captured.");
            }

            // Begin a database transaction so both updates succeed or fail together.
            await using var transaction = await _context.Database
                .BeginTransactionAsync();

            try
            {
                // Update payment only after successful signature verification.
                payment.TransactionId = request.RazorpayPaymentId;
                payment.PaymentStatus = PaymentStatus.Successful;
                payment.PaidAt = DateTime.UtcNow;

                // Confirm the associated booking.
                booking.Status = BookingStatus.Confirmed;

                // Save both tracked entities.
                await _paymentRepository.SaveChangesAsync();

                // Commit both changes together.
                await transaction.CommitAsync();

                return MapToResponse(payment);
            }
            catch
            {
                // Roll back both changes if saving or committing fails.
                await transaction.RollbackAsync();
                throw;
            }
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