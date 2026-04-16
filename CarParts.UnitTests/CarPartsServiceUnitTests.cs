using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CarParts.Common.DTOs;
using CarParts.Common.Entities;
using CarParts.DataPersistence.Repositories;
using CarParts.Services.Services;
using Moq;

namespace CarParts.UnitTests
{
    public class CarPartsServiceUnitTests
    {
        private readonly Mock<ICarPartsRepository> _mockRepository;
        private readonly ICarPartsService _service;

        public CarPartsServiceUnitTests()
        {
            _mockRepository = new Mock<ICarPartsRepository>();
            _service = new CarPartsService( _mockRepository.Object );
        }

        [Fact]
        public async Task GetById_WhenCarPartNotFound_ThrowsKeyNotFound()
        {
            // arrange
            Guid carPartId = Guid.NewGuid();
            _mockRepository.Setup(repo => repo.GetByIdAsync(carPartId))
                .ReturnsAsync((CarPart)null);

            // act
            var result = _service.GetByIdAsync(carPartId);

            // assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => result);
        }

        [Fact]
        public async Task GetById_WhenCarPartFound_ReturnsCarPart()
        {
            // arrange
            var carPartId = Guid.NewGuid();
            var carPart = new CarPart
            {
                Id = carPartId,
                Name = "test1",
                InstalledAtKm = 180_000,
                MaxKmLifetime = 120_000,
                InstalledAtDate = new DateOnly(2020, 5, 5),
                MaxMonthsLifetime = 120
            };
            _mockRepository.Setup(repo => repo.GetByIdAsync(carPartId))
                .ReturnsAsync(carPart);

            // act
            var result = await _service.GetByIdAsync(carPartId);

            // assert
            Assert.NotNull(result);
            var dto = Assert.IsType<CarPartDto>(result);

            Assert.Equal(carPart.Id, result.Id);
            Assert.Equal(carPart.Name, result.Name);
            Assert.Equal(carPart.InstalledAtKm, result.InstalledAtKm);
            Assert.Equal(carPart.MaxKmLifetime, result.MaxKmLifetime);
            Assert.Equal(carPart.InstalledAtDate, result.InstalledAtDate);
            Assert.Equal(carPart.MaxMonthsLifetime, result.MaxMonthsLifetime);
        }

        [Fact]
        public async Task GetAll_WhenListNotEmpty_ReturnsList()
        {
            // arrange
            var carParts = new List<CarPart>
            {
                new CarPart
                {
                    Id = Guid.NewGuid(),
                    Name = "test1",
                    InstalledAtKm = 180_000,
                    MaxKmLifetime = 120_000,
                    InstalledAtDate = new DateOnly(2020, 5, 5),
                    MaxMonthsLifetime = 120
                },
                new CarPart
                {
                    Id = Guid.NewGuid(),
                    Name = "test2",
                    InstalledAtKm = 200_000,
                    MaxKmLifetime = 120_000,
                    InstalledAtDate = new DateOnly(2025, 5, 5),
                    MaxMonthsLifetime = 120
                }
            };
            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(carParts);

            // act
            var result = await _service.GetAllAsync();

            // assert
            Assert.IsAssignableFrom<IEnumerable<CarPartDto>>(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task Add_ValidData_ReturnsCreatedCarPartDto()
        {
            var userId = Guid.NewGuid();
            var createDto = new CreateCarPartDto(
                "test1",
                180_000,
                120_000,
                new DateOnly(2020, 5, 5),
                120,
                userId
            );

            var savedCarPart = new CarPart
            {
                Id = Guid.NewGuid(),
                Name = "test1",
                InstalledAtKm = 180_000,
                MaxKmLifetime = 120_000,
                InstalledAtDate = new DateOnly(2020, 5, 5),
                MaxMonthsLifetime = 120,
                UserId = userId
            };
            _mockRepository.Setup(repo => repo.AddAsync(It.IsAny<CarPart>()))
                .ReturnsAsync(savedCarPart);

            // act
            var result = await _service.AddAsync(createDto);

            // assert
            Assert.NotNull(result);
            Assert.IsType<CarPartDto>(result);

            Assert.Equal(savedCarPart.Name, result.Name);
            Assert.Equal(savedCarPart.InstalledAtKm, result.InstalledAtKm);
            Assert.Equal(savedCarPart.MaxKmLifetime, result.MaxKmLifetime);
            Assert.Equal(savedCarPart.InstalledAtDate, result.InstalledAtDate);
            Assert.Equal(savedCarPart.MaxMonthsLifetime, result.MaxMonthsLifetime);
        }

        [Fact]
        public async Task Remove_WhenCarPartFound_ReturnsTrue()
        {
            Guid carPartId = Guid.NewGuid();
            var existingCarPart = new CarPart
            {
                Id = carPartId,
                Name = "test1",
                InstalledAtKm = 180_000,
                MaxKmLifetime = 120_000,
                InstalledAtDate = new DateOnly(2020, 5, 5),
                MaxMonthsLifetime = 120,
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(carPartId)).ReturnsAsync(existingCarPart);
            _mockRepository.Setup(repo => repo.RemoveAsync(carPartId)).ReturnsAsync(true);

            // act
            var result = await _service.RemoveAsync(carPartId);

            // assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.RemoveAsync(carPartId), Times.Once());
        }

        [Fact]
        public async Task Remove_WhenCarPartNotFuond_ThrowsKeyNotFound()
        {
            // arrange
            Guid carPartId = Guid.NewGuid();
            _mockRepository.Setup(repo => repo.GetByIdAsync(carPartId))
                .ReturnsAsync((CarPart)null);

            // act
            var result = _service.RemoveAsync(carPartId);

            // assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => result);
        }

        [Fact]
        public async Task Update_WhenCarPartFuond_ReturnsTrue()
        {
            // arrange
            Guid carPartId = Guid.NewGuid();
            var updateCarPartDto = new UpdateCarPartDto(
                carPartId,
                "test-update",
                180_000,
                120_000,
                new DateOnly(2020, 5, 5),
                120
            );
            var existingCarPart = new CarPart
            {
                Id = carPartId,
                Name = "test1",
                InstalledAtKm = 180_000,
                MaxKmLifetime = 120_000,
                InstalledAtDate = new DateOnly(2020, 5, 5),
                MaxMonthsLifetime = 120,
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(carPartId))
                .ReturnsAsync(existingCarPart);
            _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<CarPart>()))
                .ReturnsAsync(true);

            // act
            var result = await _service.UpdateAsync(updateCarPartDto);

            // assert
            Assert.True(result);
        }

        [Fact]
        public async Task Update_WhenCarPartNotFound_ThroesKeyNotFound()
        {
            // arrange
            Guid carPartId = Guid.NewGuid();
            var updateDto = new UpdateCarPartDto(
                carPartId,
                "test1",
                180_000,
                120_000,
                new DateOnly(2020, 5, 5),
                120
            );
            _mockRepository.Setup(repo => repo.GetByIdAsync(carPartId))
                .ReturnsAsync((CarPart)null);

            // act
            var result = _service.UpdateAsync(updateDto);

            // assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => result);
        }
    }
}
