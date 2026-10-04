using System;
using System.Collections.Generic;
using System.Text;
using YogaSphere.Domain.Enums;

namespace YogaSphere.Domain.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ScheduleId { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
