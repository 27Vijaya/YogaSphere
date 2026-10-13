using System;
using System.Collections.Generic;
using System.Text;
using YogaSphere.Domain.Enums;

namespace YogaSphere.Application.DTOs.Payment
{
    public class CreatePaymentRequest
    {
        public int BookingId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
