using System;
using System.Collections.Generic;
using System.Text;
using YogaSphere.Domain.Enums;

namespace YogaSphere.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string? TransactionId { get; set; }//provided by payment gateway(Razorpay)
        public DateTime? PaidAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Booking? Booking { get; set; }
    }
}
