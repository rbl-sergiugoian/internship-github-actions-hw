using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace CarParts.Common.Entities
{
    public class RefreshToken
    {
        [Key]
        [JsonIgnore]
        public string UserEmail { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ReplacedByToken { get; set; } = string.Empty;
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsActive => !IsExpired;
    }
}
