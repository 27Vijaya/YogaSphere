using System;
using System.Collections.Generic;
using System.Text;
using YogaSphere.Domain.Enums;

namespace YogaSphere.Application.DTOs.Booking
{
    public class UpdateBookingStatusRequest
    {
        public BookingStatus Status { get; set; }
    }
}
