using System;
using System.Collections.Generic;
using System.Text;
using YogaSphere.Domain.Entities;

namespace YogaSphere.Application.Interfaces.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(int paymentId);

        Task<IReadOnlyList<Payment>> GetByBookingIdAsync(
            int bookingId);

        Task AddAsync(Payment payment);

        Task SaveChangesAsync();
    }
}
