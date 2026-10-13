using System;
using System.Collections.Generic;
using System.Text;

namespace YogaSphere.Application.DTOs.Review
{
    public class CreateReviewRequest
    {
        public int YogaClassId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
