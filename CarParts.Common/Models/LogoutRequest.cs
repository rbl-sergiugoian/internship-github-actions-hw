using System;
using System.Collections.Generic;
using System.Text;

namespace CarParts.Common.Models
{
    public class LogoutRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
