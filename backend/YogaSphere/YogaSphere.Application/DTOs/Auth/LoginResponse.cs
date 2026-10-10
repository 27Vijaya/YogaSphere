using System;
using System.Collections.Generic;
using System.Text;

namespace YogaSphere.Application.DTOs.Auth
{
    //After successful login, the server will return authentication response to fronend
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;//JWT
        public DateTime ExpireAt { get; set; }
        public UserResponse User { get; set; } = new();//basic user info for the interface
    }
}
