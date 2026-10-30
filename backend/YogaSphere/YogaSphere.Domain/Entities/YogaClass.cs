using System;
using System.Collections.Generic;
using System.Text;

namespace YogaSphere.Domain.Entities
{
    public class YogaClass
    {
        public int Id { get; set; }
        public int YogaProgramId { get; set; }

        // Navigation property linking this class to its yoga program.
        public YogaProgram? YogaProgram { get; set; }

        public string Name { get; set; } = string.Empty;//name is required
        public string? Description { get; set; }//description is optional
        public int DurationMinutes { get; set; }
        public int Capacity { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
