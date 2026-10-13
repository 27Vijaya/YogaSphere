using System;
using System.Collections.Generic;
using System.Text;

namespace YogaSphere.Application.DTOs.Schedule
{
    public class CreateScheduleRequest
    {
        public int YogaClassId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
