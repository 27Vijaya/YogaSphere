using System;
using System.Collections.Generic;
using System.Text;

namespace YogaSphere.Application.DTOs.YogaProgram
{
    public class UpdateYogaProgramRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationWeeks { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
    }
}
