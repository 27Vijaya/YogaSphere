using System;
using System.Collections.Generic;
using System.Text;

namespace YogaSphere.Application.DTOs.Schedule
{
    public class ScheduleResponse
    {
        public int Id { get; set; }
        public int YogaClassId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
