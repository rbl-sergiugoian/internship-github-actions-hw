using System;
using System.Collections.Generic;
using System.Text;
using CarParts.Common.Entities;

namespace CarParts.DataPersistence.Repositories
{
    public interface ICarPartsRepository
    {
        Task<IEnumerable<CarPart>> GetAllAsync();
        Task<CarPart?> GetByIdAsync(Guid id);
        Task<CarPart> AddAsync(CarPart carPart);
        Task<bool> RemoveAsync(Guid id);
        Task<bool> UpdateAsync(CarPart carPart);
    }
}
