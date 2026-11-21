using YogaSphere.Application.DTOs.Payment;

namespace YogaSphere.Application.Interfaces
{
    public interface IPaymentService
    {
        // Starts the payment process for a customer's booking.
        Task<PaymentInitiationResponse> InitiateAsync(int customerId,CreatePaymentRequest request);

        // Retrieves a payment only if it belongs to the customer.
        Task<PaymentResponse?> GetByIdAsync(int customerId,int paymentId);

        // Retrieves payment records only for the customer's booking.
        Task<IReadOnlyList<PaymentResponse>> GetByBookingIdAsync(int customerId,int bookingId);

        Task<PaymentResponse?> VerifyPaymentAsync(int customerId,VerifyPaymentRequest request);
    }
}