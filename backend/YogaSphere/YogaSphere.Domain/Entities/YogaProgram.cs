using System;
using System.Collections.Generic;
using System.Text;

namespace YogaSphere.Domain.Entities
{
    internal class YogaProgram
    {
        public int Id { get; set; }
        public int TrainerId { get; set; } //connect program to trainer
        public string Name { get; set; } = string.Empty;//program name
        public string? Description { get; set; }
        public int DurationWeeks { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
