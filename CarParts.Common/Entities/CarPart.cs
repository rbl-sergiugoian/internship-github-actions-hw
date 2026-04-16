using System;
using System.Collections.Generic;
using System.Text;

namespace CarParts.Common.Entities
{
    public class CarPart
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int InstalledAtKm { get; set; }
        public int MaxKmLifetime { get; set; }
        public DateOnly InstalledAtDate { get; set; }
        public int MaxMonthsLifetime { get; set; }

        public Guid UserId { get; set; }
    }
}
