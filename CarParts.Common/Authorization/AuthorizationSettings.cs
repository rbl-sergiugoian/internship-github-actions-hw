using System;
using System.Collections.Generic;
using System.Text;

namespace CarParts.Common.Authorization
{
    public class AuthorizationSettings
    {
        public string Secret { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int AccessTokenTTLMinutes { get; set; }
        public int RefreshTokenTTLDays { get; set; }
    }
}
