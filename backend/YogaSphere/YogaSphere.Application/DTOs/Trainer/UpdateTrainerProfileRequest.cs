using System;
using System.Collections.Generic;
using System.Text;

namespace YogaSphere.Application.DTOs.Trainer
{
    public class UpdateTrainerProfileRequest
    {
        public string? Specialization { get; set; }
        public int? ExperienceYears { get; set; }
        public string? Certification { get; set; }
        public string? Bio { get; set; }
    }
}
