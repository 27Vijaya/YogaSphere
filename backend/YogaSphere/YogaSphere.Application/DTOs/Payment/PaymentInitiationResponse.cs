using System;
using System.Collections.Generic;
using System.Text;

namespace YogaSphere.Application.DTOs.Payment
{
    public class PaymentInitiationResponse
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public string GatewayOrderId { get; set; } = string.Empty;//order id returned by payment gateway
        public string GatewayName { get; set; } = string.Empty;//payment gateway name(like Razorpay)

        public string KeyId { get; set; } = string.Empty;// Public Razorpay Key ID used by the frontend Checkout.
    }
}
