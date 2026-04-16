using System;
using System.Collections.Generic;
using System.Text;
using CarParts.Common.DTOs;
using CarParts.Common.Entities;
using CarParts.Common.Utils;
using CarParts.DataPersistence.Repositories;

namespace CarParts.Services.Services
{
    public class CarPartsService : ICarPartsService
    {
        private readonly ICarPartsRepository _carPartsRepository;

        public CarPartsService(ICarPartsRepository carPartsRepository)
        {
            _carPartsRepository = carPartsRepository;
        }

        public async Task<IEnumerable<CarPartDto>> GetAllAsync()
        {
            var carParts = await _carPartsRepository.GetAllAsync();

            return carParts.Select(part =>
                part.ToDto()
            );
        }

        public async Task<CarPartDto?> GetByIdAsync(Guid id)
        {
            var carPart = await _carPartsRepository.GetByIdAsync(id);
            if (carPart == null)
            {
                throw new KeyNotFoundException($"car componet with id {id} not found");
            }

            return carPart.ToDto();
        }

        public async Task<CarPartDto> AddAsync(CreateCarPartDto createCarPartDto)
        {
            var createdCarPart = await _carPartsRepository.AddAsync(createCarPartDto.ToEntity());
            return createdCarPart.ToDto();
        }

        public async Task<bool> RemoveAsync(Guid id)
        {
            var existingCarPart = await _carPartsRepository.GetByIdAsync(id);
            if (existingCarPart == null)
            {
                throw new KeyNotFoundException($"cant remove component with id : {id}, because doesnt already exist");
            }

            return await _carPartsRepository.RemoveAsync(id);
        }

        public async Task<bool> UpdateAsync(UpdateCarPartDto updateCarPartDto)
        {
            var existingCarPart = await _carPartsRepository.GetByIdAsync(updateCarPartDto.Id);
            if (existingCarPart == null)
            {
                throw new KeyNotFoundException($"cant remove component with id : {updateCarPartDto.Id}, because doesnt already exist");
            }

            var carPart = updateCarPartDto.ToEntity();
            return await _carPartsRepository.UpdateAsync(carPart);
        }
    }
}
