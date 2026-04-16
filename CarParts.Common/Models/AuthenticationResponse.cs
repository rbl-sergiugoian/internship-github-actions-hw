using System;
using System.Collections.Generic;
using System.Text;

namespace CarParts.Common.Models
{
    public class AuthenticationResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
