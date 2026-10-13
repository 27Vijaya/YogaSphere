using System;
using System.Collections.Generic;
using System.Text;

namespace YogaSphere.Application.DTOs.Customer
{
    public class UpdateCustomerProfileRequest
    {
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? EmergencyContact { get; set; }
    }
}
