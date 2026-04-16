using System;
using System.Collections.Generic;
using System.Text;

namespace CarParts.Common.DTOs
{
    public record CarPartDto(
        Guid Id,
        string Name,
        int InstalledAtKm,
        int MaxKmLifetime,
        DateOnly InstalledAtDate,
        int MaxMonthsLifetime
    );
}
