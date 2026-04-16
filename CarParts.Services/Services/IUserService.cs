using System;
using System.Collections.Generic;
using System.Text;
using CarParts.Common.Entities;
using CarParts.Common.Models;

namespace CarParts.Services.Services
{
    public interface IUserService
    {
        AuthenticationResponse Login(LoginRequest loginRequest);
        AuthenticationResponse RenewAccessToken(string accessToken);
        AuthenticationResponse RefreshAccessToken(string refreshToken);
        void Logout(string refreshToken);
        RegistrationResponse Register(RegisterRequest request);
        User GetByEmail(string email);
        User GetById(Guid id);
    }
}
