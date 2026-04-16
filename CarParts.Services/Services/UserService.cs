using System;
using System.Collections.Generic;
using System.Text;
using CarParts.Common.Entities;
using CarParts.Common.Models;
using CarParts.Common.Utils;
using CarParts.Services.Auth;

namespace CarParts.Services.Services
{
    public class UserService : IUserService
    {
        private IJwtUtils _jwtUtils;
        private List<User> _users = new List<User>();

        public UserService(IJwtUtils jwtUtils)
        {
            _jwtUtils = jwtUtils;

            _users.Add(new User
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                Email = "admin@gmail.com",
                PasswordHash = _jwtUtils.HashPassword("admin"),
                UserRole = "Admin",
                RefreshTokens = new List<RefreshToken>()
            });
            _users.Add(new User
            {
                Id = Guid.NewGuid(),
                Username = "user1",
                Email = "user1@yahoo.com",
                PasswordHash = _jwtUtils.HashPassword("pass1"),
                UserRole = "User",
                RefreshTokens = new List<RefreshToken>()
            });
        }

        public RegistrationResponse Register(RegisterRequest request)
        {
            if (_users.Any(user => user.Email == request.Email))
            {
                throw new ArgumentException("email already exists");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                PasswordHash = _jwtUtils.HashPassword(request.Password),
                UserRole = "User",
                RefreshTokens = new List<RefreshToken>()
            };

            _users.Add(user);
            return user.ToModel();
        }

        public AuthenticationResponse Login(LoginRequest loginRequest)
        {
            var user = _users.FirstOrDefault(user => user.Username == loginRequest.Username);
            if (user == null)
            {
                throw new UnauthorizedAccessException("no user found");
            }

            var incomingHash = _jwtUtils.HashPassword(loginRequest.Password);
            if (user.PasswordHash != incomingHash)
            {
                Console.WriteLine(user.PasswordHash, incomingHash);
                throw new UnauthorizedAccessException("invalid login credentials");
            }

            var accessToken = _jwtUtils.GenerateAccessToken(user);
            var refreshToken = _jwtUtils.GenerateRefreshToken(user.Email);

            user.RefreshTokens.Add(refreshToken);

            return new AuthenticationResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
        }

        public User GetByEmail(string email)
        {
            return _users.FirstOrDefault(user => user.Email == email);
        }

        public User GetById(Guid id)
        {
            return _users.FirstOrDefault(user => user.Id == id);
        }

        public void Logout(string refreshToken)
        {
            var user = _users.FirstOrDefault(user => user.RefreshTokens.Any(refToken => refToken.Token == refreshToken));
            if (user != null)
            {
                var tokenToRemove = user.RefreshTokens.First(refToken => refToken.Token == refreshToken);
                user.RefreshTokens.Remove(tokenToRemove);
            }
        }

        public AuthenticationResponse RefreshAccessToken(string refreshToken)
        {
            var user = _users.FirstOrDefault(user => user.RefreshTokens.Any(refToken => refToken.Token == refreshToken));
            if (user == null)
            {
                throw new UnauthorizedAccessException($"invalid refresh token: {refreshToken}");
            }

            var existingTOken = user.RefreshTokens.Single(refToken => refToken.Token == refreshToken);
            if (DateTime.UtcNow >= existingTOken.ExpiresAt)
            {
                user.RefreshTokens.Remove(existingTOken);
                throw new UnauthorizedAccessException("refresh token has just expired, login required");
            }

            var newAccessToken = _jwtUtils.GenerateAccessToken(user);
            var newRefreshToken = _jwtUtils.GenerateRefreshToken(user.Email);

            user.RefreshTokens.Remove(existingTOken);
            user.RefreshTokens.Add(newRefreshToken);

            return new AuthenticationResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            };
        }

        
        public AuthenticationResponse RenewAccessToken(string accessToken)
        {
            var email = _jwtUtils.ValidateJwtToken(accessToken);
            if (email == null)
            {
                throw new UnauthorizedAccessException("invalid or expired access token");
            }

            var user = GetByEmail(email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("user with given email not found");
            }

            var newAccessToken = _jwtUtils.GenerateAccessToken(user);

            return new AuthenticationResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = user.RefreshTokens.LastOrDefault().Token
            };
        }
    }
}
