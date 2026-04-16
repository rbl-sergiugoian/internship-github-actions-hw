using System;
using System.Collections.Generic;
using System.Text;

namespace CarParts.Common.Models
{
    public class ValidationErrorResponse
    {
        public Dictionary<string, string[]> Errors { get; set; }
    }
}
