using System;
using System.Collections.Generic;
using System.Text;

namespace YogaSphere.Application.DTOs.YogaClass
{
    public class UpdateYogaClassRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationMinutes { get; set; }
        public int Capacity { get; set; }
        public bool IsActive { get; set; }
    }
}
