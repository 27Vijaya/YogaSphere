using System;
using System.Collections.Generic;
using System.Text;
using YogaSphere.Application.DTOs.Auth;

namespace YogaSphere.Application.Interfaces
{
    public interface IAuthService
    {
        //accept registration details and returns newly registered user's details
        Task<UserResponse> RegisterAsync(RegisterRequest request);

        //Verifies credentials and returns login response with access token and user details
        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
}
