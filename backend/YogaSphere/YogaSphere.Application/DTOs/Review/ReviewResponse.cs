using System;
using System.Collections.Generic;
using System.Text;

namespace YogaSphere.Application.DTOs.Review
{
    public class ReviewResponse
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int YogaClassId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
