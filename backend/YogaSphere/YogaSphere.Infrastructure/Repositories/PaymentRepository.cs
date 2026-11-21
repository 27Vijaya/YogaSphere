using Microsoft.EntityFrameworkCore;
using YogaSphere.Application.Interfaces.Repositories;
using YogaSphere.Domain.Entities;
using YogaSphere.Infrastructure.Persistence;

namespace YogaSphere.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly YogaSphereDbContext _context;

        public PaymentRepository(YogaSphereDbContext context)
        {
            _context = context;
        }

        // Retrieves a payment by ID. Tracking is enabled because
        // the payment status may need to be updated later.
        public async Task<Payment?> GetByIdAsync(int paymentId)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(payment => payment.Id == paymentId);
        }

        // Retrieves a payment using its Razorpay order ID.
        // Tracking is enabled because webhook processing may update its status.
        public async Task<Payment?> GetByGatewayOrderIdAsync(
            string gatewayOrderId)
        {
            return await _context.Payments
                .Include(payment => payment.Booking)
                .FirstOrDefaultAsync(payment => payment.GatewayOrderId == gatewayOrderId);
        }

        // Retrieves all payment attempts associated with a booking.
        public async Task<IReadOnlyList<Payment>> GetByBookingIdAsync(
            int bookingId)
        {
            return await _context.Payments
                .AsNoTracking()
                .Where(payment => payment.BookingId == bookingId)
                .OrderByDescending(payment => payment.CreatedAt)
                .ToListAsync();
        }

        // Adds a new payment to the database context.
        public async Task AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
        }

        // Saves pending changes to the database.
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}