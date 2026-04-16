using System;
using System.Collections.Generic;
using System.Text;

namespace CarParts.Common.Models
{
    public record RegistrationResponse(
        Guid Id,
        string Username,
        string Email
    );
}
