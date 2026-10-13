using System;
using System.Collections.Generic;
using System.Text;

namespace YogaSphere.Application.DTOs.Trainer
{
    public class TrainerProfileResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Specialization { get; set; }
        public int? ExperienceYears { get; set; }
        public string? Certification { get; set; }
        public string? Bio { get; set; }//trainer introduction
    }
}
