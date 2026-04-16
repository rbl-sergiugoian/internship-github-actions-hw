using System;
using System.Collections.Generic;
using System.Text;

namespace CarParts.Common.DTOs
{
    public record CreateCarPartDto(
        string Name,
        int InstalledAtKm,
        int MaxKmLifetime,
        DateOnly InstalledAtDate,
        int MaxMonthsLifetime,
        Guid userId
    );
}
