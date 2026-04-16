using System;
using System.Collections.Generic;
using System.Text;
using CarParts.Common.Entities;

namespace CarParts.DataPersistence.Repositories
{
    public class CarPartsRepository : ICarPartsRepository
    {
        private IList<CarPart> _carParts;

        public CarPartsRepository()
        {
            _carParts = GetHardcodedList();
        }

        public async Task<IEnumerable<CarPart>> GetAllAsync()
        {
            return _carParts;
        }

        public async Task<CarPart?> GetByIdAsync(Guid id)
        {
            return _carParts.FirstOrDefault(part => part.Id == id);
        }

        public async Task<CarPart> AddAsync(CarPart carPart)
        {
            carPart.Id = Guid.NewGuid();
            _carParts.Add(carPart);
            return carPart;
        }

        public async Task<bool> RemoveAsync(Guid id)
        {
            var existingCarPart = _carParts.FirstOrDefault(part =>
                part.Id == id
            );
            if (existingCarPart == null)
            {
                return false;
            }

            return _carParts.Remove(existingCarPart);
        }

        public async Task<bool> UpdateAsync(CarPart carPart)
        {
            var existingCarPart = _carParts.FirstOrDefault(part =>
                part.Id == carPart.Id
            );
            if (existingCarPart == null)
            {
                return false;
            }

            existingCarPart.Name = carPart.Name;
            existingCarPart.InstalledAtKm = carPart.InstalledAtKm;
            existingCarPart.MaxKmLifetime = carPart.MaxKmLifetime;
            existingCarPart.InstalledAtDate = carPart.InstalledAtDate;
            existingCarPart.MaxMonthsLifetime = carPart.MaxMonthsLifetime;

            return true;
        }

        private IList<CarPart> GetHardcodedList()
        {
            return new List<CarPart>
            {
                new CarPart
                {
                    Id = Guid.Parse("3f2504e0-4f89-11d3-9a0c-0305e82c3301"),
                    Name = "intercooler",
                    InstalledAtKm = 300_000,
                    MaxKmLifetime = 250_000,
                    InstalledAtDate = new DateOnly(2020, 5, 15),
                    MaxMonthsLifetime = 120,
                },
                new CarPart
                {
                    Id = Guid.Parse("3f2504e0-4f89-11d3-9a0c-0305e82c3302"),
                    Name = "oil filter",
                    InstalledAtKm = 430_000,
                    MaxKmLifetime = 15_000,
                    InstalledAtDate = new DateOnly(2024, 1, 10),
                    MaxMonthsLifetime = 12
                }
            };
        }
    }
}
