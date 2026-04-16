using CarParts.Common.Entities;

namespace CarParts.Services.Auth
{
    public interface IJwtUtils
    {
        string GenerateAccessToken(User user);
        string? ValidateJwtToken(string token);
        RefreshToken GenerateRefreshToken(string userEmail);
        string HashPassword(string password);
    }
}
