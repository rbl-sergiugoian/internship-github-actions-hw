using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CarParts.Common.Models
{
    public record CarPartRequest(
        [Required(ErrorMessage = "name is required")]
        [MinLength(2, ErrorMessage = "length must be at least 2 characters")]
        [MaxLength(50)]
        string Name,

        [Range(0, 5_000_000, ErrorMessage = "please enter valid nr of kilometers")]
        int InstalledAtKm,

        [Range(100, 1_000_000, ErrorMessage = "lifetime must be positive")]
        int MaxKmLifetime,

        [Required(ErrorMessage = "installation date is required")]
        DateOnly InstalledAtDate,

        [Range(1, 1_000, ErrorMessage = "lifetime in months must be between 1 and 1000")]
        int MaxMonthsLifetime
    );
}
