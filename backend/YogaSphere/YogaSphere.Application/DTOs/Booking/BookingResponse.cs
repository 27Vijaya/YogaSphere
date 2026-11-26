
using YogaSphere.Domain.Enums;

namespace YogaSphere.Application.DTOs.Booking
{
    public class BookingResponse
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ScheduleId { get; set; }

        // Details of the scheduled yoga class.
        public int YogaClassId { get; set; }
        public string YogaClassName { get; set; } = string.Empty;
        public DateTime ClassDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public BookingStatus Status { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
