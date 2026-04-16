using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CarParts.Common.Authorization;
using CarParts.Common.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CarParts.Services.Auth
{
    public class JwtUtils : IJwtUtils
    {
        private readonly AuthorizationSettings _authorizationSettings;

        public JwtUtils(IOptions<AuthorizationSettings> authorizationSettings)
        {
            _authorizationSettings = authorizationSettings.Value;
        }

        public string GenerateAccessToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_authorizationSettings.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.Role, user.UserRole),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _authorizationSettings.Issuer,
                audience: _authorizationSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_authorizationSettings.AccessTokenTTLMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string? ValidateJwtToken(string token)
        {
            if (token == null)
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_authorizationSettings.Secret);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _authorizationSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _authorizationSettings.Audience,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userEmail = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "sub" || claim.Type == JwtRegisteredClaimNames.Sub)?.Value;

                return userEmail;
            }
            catch
            {
                return null;
            }
        }

        public RefreshToken GenerateRefreshToken(string email)
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);

            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserEmail = email
            };

            return refreshToken;
        }

        public string HashPassword(string password)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.UTF8.GetBytes("salt"),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }
    }
}
