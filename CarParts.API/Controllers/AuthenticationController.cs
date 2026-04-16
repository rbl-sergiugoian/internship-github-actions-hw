using System.Reflection;
using CarParts.Common.Models;
using CarParts.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarParts.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthenticationController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            var response = _userService.Login(loginRequest);
            return Ok(response);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest registerRequest)
        {
            var response = _userService.Register(registerRequest);
            return Ok(response);
        }

        [HttpPost("renew-access-token")]
        public IActionResult RenewValidAccessToken([FromBody] RenewTokenRequest renewTokenRequest)
        {
            var reponse = _userService.RenewAccessToken(renewTokenRequest.AccessToken);
            return Ok(reponse);
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            var response = _userService.RefreshAccessToken(refreshTokenRequest.RefreshToken);
            return Ok(response);
        }

        [HttpPost("logout")]
        public IActionResult Logout([FromBody] LogoutRequest logoutRequest)
        {
            _userService.Logout(logoutRequest.RefreshToken);
            return Ok(new { message = "logged out succwssfully" });
        }
    }
}
