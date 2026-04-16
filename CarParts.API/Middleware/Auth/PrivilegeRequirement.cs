using Microsoft.AspNetCore.Authorization;

namespace CarParts.API.Middleware.Authentication
{
    public class PrivilegeRequirement : IAuthorizationRequirement
    {
        public string Role { get; private set; } = string.Empty;

        public PrivilegeRequirement(string role)
        {
            Role = role;
        }
    }
}
