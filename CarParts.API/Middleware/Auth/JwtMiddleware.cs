using CarParts.API.Middleware.Authentication;
using CarParts.Common.Authorization;
using CarParts.Services.Auth;
using CarParts.Services.Services;
using Microsoft.Extensions.Options;

namespace CarParts.API.Middleware.Auth
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AuthorizationSettings _authSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<AuthorizationSettings> authSettings)
        {
            _next = next;
            _authSettings = authSettings.Value;
        }

        public async Task Invoke(HttpContext context, IJwtUtils jwtUtils, IUserService userService)
        {
            var token = context.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();

            var email = jwtUtils.ValidateJwtToken(token);

            if (email != null)
            {
                var user = userService.GetByEmail(email);
                context.Items["User"] = user;
            }

            await _next(context);
        }
    }
}
