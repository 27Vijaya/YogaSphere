using System;
using System.Collections.Generic;
using System.Text;

namespace YogaSphere.Application.DTOs.Schedule
{
    public class UpdateScheduleRequest
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsActive { get; set; }
    }
}
