using System;
using System.Collections.Generic;
using System.Text;
using CarParts.Common.DTOs;

namespace CarParts.Services.Services
{
    public interface ICarPartsService
    {
        Task<IEnumerable<CarPartDto>> GetAllAsync();
        Task<CarPartDto?> GetByIdAsync(Guid id);
        Task<CarPartDto> AddAsync(CreateCarPartDto createCarPartDto);
        Task<bool> RemoveAsync(Guid id);
        Task<bool> UpdateAsync(UpdateCarPartDto updateCarPartDto);
    }
}
