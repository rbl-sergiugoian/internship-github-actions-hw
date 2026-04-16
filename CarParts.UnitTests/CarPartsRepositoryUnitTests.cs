using System;
using System.Collections.Generic;
using System.Text;
using CarParts.Common.Entities;
using CarParts.DataPersistence.Repositories;

namespace CarParts.UnitTests
{
    public class CarPartsRepositoryUnitTests
    {
        private readonly ICarPartsRepository _repository;

        public CarPartsRepositoryUnitTests()
        {
            _repository = new CarPartsRepository();
        }

        [Fact]
        public async Task GetAll_ReturnsHardcodedList()
        {
            // act
            var result = await _repository.GetAllAsync();

            // assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetById_WhenCarPartFound_ReturnsCarPart()
        {
            var existingId = Guid.Parse("3f2504e0-4f89-11d3-9a0c-0305e82c3301");

            // act
            var result = await _repository.GetByIdAsync(existingId);

            // assert
            Assert.NotNull(result);
            Assert.Equal("intercooler", result.Name);
        }

        [Fact]
        public async Task GetById_WhenCarPartNotFound_ReturnsNull()
        {
            var fakeId = Guid.NewGuid();

            // act
            var result = await _repository.GetByIdAsync(fakeId);

            // assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Add_ValidCarPart_IncreasesCountAndAssignsGuid()
        {
            // arrange
            var newPart = new CarPart
            {
                Name = "test1",
                InstalledAtKm = 180_000,
                MaxKmLifetime = 120_000,
                InstalledAtDate = new DateOnly(2025, 5, 5),
                MaxMonthsLifetime = 120
            };

            // act
            var result = await _repository.AddAsync(newPart);
            var allParts = await _repository.GetAllAsync();

            // assert
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal(3, allParts.Count());
            Assert.Equal("test1", result.Name);
        }

        [Fact]
        public async Task Remove_WhenCarPartFound_ReturnsTrueAndDecreasesCount()
        {
            // arrange
            var carPartId = Guid.Parse("3f2504e0-4f89-11d3-9a0c-0305e82c3302");

            // act
            var result = await _repository.RemoveAsync(carPartId);
            var allParts = await _repository.GetAllAsync();

            // assert
            Assert.True(result);
            Assert.Single(allParts);
        }

        [Fact]
        public async Task Remove_WhenCarPartNotFOund_ReturnsFalse()
        {
            // arrange
            var fakeId = Guid.NewGuid();

            // act
            var result = await _repository.RemoveAsync(fakeId);

            // assert
            Assert.False(result);
        }

        [Fact]
        public async Task Update_WhenCarPartFound_UpdatesDataAndReturnsTrue()
        {
            // arrange
            var carPartId = Guid.Parse("3f2504e0-4f89-11d3-9a0c-0305e82c3301");
            var updatedInfo = new CarPart
            {
                Id = carPartId,
                Name = "test-updated",
                InstalledAtKm = 800_000,
                MaxKmLifetime = 250_000,
                InstalledAtDate = new DateOnly(2020, 5, 15),
                MaxMonthsLifetime = 120
            };

            // act
            var result = await _repository.UpdateAsync(updatedInfo);
            var updatedPart = await _repository.GetByIdAsync(carPartId);

            // assert
            Assert.True(result);
            Assert.NotNull(updatedPart);
            Assert.Equal("test-updated", updatedPart.Name);
            Assert.Equal(800_000, updatedPart.InstalledAtKm);
        }

        [Fact]
        public async Task Update_WhenCarPartNotFound_ReturnsFalse()
        {
            // arrange
            var fakeId = Guid.NewGuid();
            var fakePart = new CarPart 
            { 
                Id = fakeId, 
                Name = "test1",
                InstalledAtKm = 180_000,
                MaxKmLifetime = 120_000,
                InstalledAtDate = new DateOnly(2025, 5, 5),
                MaxMonthsLifetime = 120
            };

            // act
            var result = await _repository.UpdateAsync(fakePart);

            // assert
            Assert.False(result);
        }
    }
}
