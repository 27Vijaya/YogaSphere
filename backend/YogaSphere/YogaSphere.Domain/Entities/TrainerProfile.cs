using System;
using System.Collections.Generic;
using System.Text;

namespace YogaSphere.Domain.Entities
{
    public class TrainerProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; } //connect profile to user

        // Navigation property linking this profile to its user account.
        public User? User { get; set; }

        public string? Spacialization { get; set; }
        public int? Experience { get; set; }
        public string? Certifications { get; set; }
        public string? Bio { get; set; }//trainer's introduction
    }
}
