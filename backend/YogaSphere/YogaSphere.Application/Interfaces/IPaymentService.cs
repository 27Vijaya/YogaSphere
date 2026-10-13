using System;
using System.Collections.Generic;
using System.Text;
using YogaSphere.Application.DTOs.Payment;

namespace YogaSphere.Application.Interfaces
{
    public interface IPaymentService
    {
        //starts the payment process for a customer's booking.
        Task<PaymentInitiationResponse> InitiateAsync(
        int customerId,
        CreatePaymentRequest request);

        //retrieves a payment by its ID
        Task<PaymentResponse?> GetByIdAsync(int paymentId);

        //retrieves payment records associated with a booking.
        Task<IReadOnlyList<PaymentResponse>> GetByBookingIdAsync(
            int bookingId);
    }
}
