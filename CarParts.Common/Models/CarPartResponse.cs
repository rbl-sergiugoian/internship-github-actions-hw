using System;
using System.Collections.Generic;
using System.Text;

namespace CarParts.Common.Models
{
    public record CarPartResponse(
        Guid Id,
        string Name,
        int InstalledAtKm,
        int MaxKmLifetime,
        DateOnly InstalledAtDate,
        int MaxMonthsLifetime
    );
}
