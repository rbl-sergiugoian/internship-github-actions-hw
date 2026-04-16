using System;
using System.Collections.Generic;
using System.Text;

namespace CarParts.ConsoleUI
{
    public class AuthState
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
