using System.ComponentModel.DataAnnotations;
using CarParts.API.Controllers;
using CarParts.Common.DTOs;
using CarParts.Common.Entities;
using CarParts.Common.Models;
using CarParts.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CarParts.UnitTests;

public class CarPartsControllerUnitTests
{
    private readonly Mock<ICarPartsService> _mockCarPartsService;
    private readonly CarPartsController _controller;

    public CarPartsControllerUnitTests()
    {
        _mockCarPartsService = new Mock<ICarPartsService>();
        _controller = new CarPartsController(_mockCarPartsService.Object);
    }

    [Fact]
    public async Task GetById_WhenCarPartFound_ReturnsCarPart()
    {
        // arrange
        var carPartDto = new CarPartDto(
            Guid.NewGuid(),
            "test1",
            180_000,
            120_000,
            new DateOnly(2020, 5, 5),
            120
        );
        _mockCarPartsService.Setup(service => service.GetByIdAsync(carPartDto.Id))
            .ReturnsAsync(carPartDto);

        // act
        var rwsult = await _controller.GetById(carPartDto.Id);

        // assert
        var okResult = Assert.IsType<OkObjectResult>(rwsult);
        var model = Assert.IsType<CarPartResponse>(okResult.Value);

        Assert.Equal(carPartDto.Id, model.Id);
        Assert.Equal(carPartDto.Name, model.Name);
        Assert.Equal(carPartDto.InstalledAtKm, model.InstalledAtKm);
        Assert.Equal(carPartDto.MaxKmLifetime, model.MaxKmLifetime);
        Assert.Equal(carPartDto.InstalledAtDate, model.InstalledAtDate);
        Assert.Equal(carPartDto.MaxMonthsLifetime, model.MaxMonthsLifetime);
    }

    [Fact]
    public async Task GetById_WhenCarPartNotFound_ReturnsNotFound()
    {
        // arrange
        Guid carPartId = Guid.NewGuid();
        _mockCarPartsService.Setup(servicee => servicee.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((CarPartDto)null);

        // act
        var notFoundRwsult = await _controller.GetById(carPartId);

        // assert
        Assert.IsType<NotFoundObjectResult>(notFoundRwsult);

        _mockCarPartsService.Verify(service => service.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task GetById_WithEmptyId_ReturnsBadRequest()
    {
        // arrange
        Guid carPartId = Guid.Empty;

        // act
        var badRequestResult = await _controller.GetById(carPartId);

        // assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(badRequestResult);
        Assert.Equal("id cant be empty", badRequest.Value);

        _mockCarPartsService.Verify(service => service.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task GetAll_WhenEmptyList_ReturnsSuccessNoContent()
    {
        // arrange
        _mockCarPartsService.Setup(service => service.GetAllAsync())
                .ReturnsAsync(new List<CarPartDto>());

        // act
        var noCOntentResult = await _controller.GetAll();

        // assert
        Assert.IsType<NoContentResult>(noCOntentResult);
    }

    [Fact]
    public async Task GetAll_WhenNotEmpty_ReturnsSuccessWithListOfCarParts()
    {
        // arrange
        var carParts = new List<CarPartDto>
        {
            new CarPartDto(
                Guid.NewGuid(),
                "test1",
                180_000,
                120_000,
                new DateOnly(2020, 5, 5),
                120
            ),
            new CarPartDto(
                Guid.NewGuid(),
                "test2",
                200_000,
                120_000,
                new DateOnly(2025, 5, 5),
                120
            ),
        };

        _mockCarPartsService.Setup(service => service.GetAllAsync())
            .ReturnsAsync(carParts);

        // act
        var result = await _controller.GetAll();

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<CarPartResponse>>(ok.Value);

        Assert.Equal(2, model.Count());
    }



    [Fact]
    public async Task Add_ValidRequestData_ReturnsCreatedCarPart()
    {
        // arrange
        var mockUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "test",
            Email = "test@yahoo.com",
        };

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext()
        };

        _controller.HttpContext.Items["User"] = mockUser;

        var request = new CarPartRequest(
            "test1",
            180_000,
            120_000,
            new DateOnly(2020, 5, 5),
            120
        );

        var createdDto = new CarPartDto(
            Guid.NewGuid(),
            request.Name,
            request.InstalledAtKm,
            request.MaxKmLifetime,
            request.InstalledAtDate,
            request.MaxMonthsLifetime
        );

        _mockCarPartsService.Setup(srrvice => srrvice.AddAsync(It.IsAny<CreateCarPartDto>()))
            .ReturnsAsync(createdDto);

        // act
        var result = await _controller.Add(request);

        // asesrt
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var model = Assert.IsType<CarPartResponse>(createdResult.Value);

        Assert.NotNull(model);
        Assert.Equal(request.Name, model.Name);
        Assert.Equal(request.InstalledAtKm, model.InstalledAtKm);
        Assert.Equal(request.MaxKmLifetime, model.MaxKmLifetime);
        Assert.Equal(request.InstalledAtDate, model.InstalledAtDate);
        Assert.Equal(request.MaxMonthsLifetime, model.MaxMonthsLifetime);

        _mockCarPartsService.Verify(service => service.AddAsync(It.IsAny<CreateCarPartDto>()), Times.Once);
    }

    [Fact]
    public async Task Add_WithInvalidRequestData_ReturnsBadRequest()
    {
        // arrange
        var invalidRequest = new CarPartRequest(
            string.Empty,
            180_000,
            120_000,
            new DateOnly(2020, 5, 5),
            120
        );

        _controller.ModelState.AddModelError("Name", "name is required");

        // act
        var result = await _controller.Add(invalidRequest);

        // asesrt
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);

        _mockCarPartsService.Verify(service => service.AddAsync(It.IsAny<CreateCarPartDto>()), Times.Never);
    }

    [Fact]
    public async Task Remove_WithEMptyId_ReturnsBadRequest()
    {
        // arrqnge
        Guid carPartId = Guid.Empty;

        // act
        var result = await _controller.Remove(carPartId);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);

        _mockCarPartsService.Verify(service => service.RemoveAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Remove_WhenCarPartRemoved_ReturnsSuccess()
    {
        // arrqnge
        _mockCarPartsService.Setup(s => s.RemoveAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true);

        // act
        var result = await _controller.Remove(Guid.NewGuid());

        // assert
        Assert.IsType<OkResult>(result);

        _mockCarPartsService.Verify(service => service.RemoveAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task Remove_WhenCarPartNotRemoved_ReturnsServeRError()
    {
        // arrqnge
        _mockCarPartsService.Setup(s => s.RemoveAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(false);

        // act
        var result = await _controller.Remove(Guid.NewGuid());

        // assert
        var serverErrorResponse = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(500, serverErrorResponse.StatusCode);

        _mockCarPartsService.Verify(service => service.RemoveAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task Remove_WhenCarPartNotFound_ThrowsKeyNotFound()
    {
        // arrange
        Guid carPartId = Guid.NewGuid();

        _mockCarPartsService.Setup(service => service.RemoveAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new KeyNotFoundException("not found"));

        // act
        var result = _controller.Remove(carPartId);

        // assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => result);
    }

    [Fact]
    public async Task Update_WhenValidRequestData_ReturnsOkAndUpdatesCarPart()
    {
        // arrange
        Guid carPartId = Guid.NewGuid();
        var request = new CarPartRequest(
            string.Empty,
            180_000,
            120_000,
            new DateOnly(2020, 5, 5),
            120
        );

        _mockCarPartsService.Setup(service => service.UpdateAsync(It.IsAny<UpdateCarPartDto>()))
            .ReturnsAsync(true);

        // act
        var result = await _controller.Update(carPartId, request);

        // assert
        Assert.IsType<OkResult>(result);
        _mockCarPartsService.Verify(service => service.UpdateAsync(It.Is<UpdateCarPartDto>(dto =>
            dto.Id == carPartId &&
            dto.Name == request.Name &&
            dto.InstalledAtKm == request.InstalledAtKm &&
            dto.MaxKmLifetime == request.MaxKmLifetime &&
            dto.InstalledAtDate == request.InstalledAtDate &&
            dto.MaxMonthsLifetime == request.MaxMonthsLifetime
        )), Times.Once);
    }

    [Fact]
    public async Task Update_WhenInvalidRequestData_ReturnsBadRequest()
    {
        // arrange
        Guid carPartId = Guid.NewGuid();
        var invalidRequest = new CarPartRequest(
            string.Empty,
            180_000,
            120_000,
            new DateOnly(2020, 5, 5),
            120
        );

        _controller.ModelState.AddModelError("Name", "name is required");
        _mockCarPartsService.Setup(service => service.UpdateAsync(It.IsAny<UpdateCarPartDto>()))
            .ReturnsAsync(false);

        // act
        var result = await _controller.Update(carPartId, invalidRequest);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
        _mockCarPartsService.Verify(service => service.UpdateAsync(It.IsAny<UpdateCarPartDto>()), Times.Never);
    }

    [Fact]
    public async Task Update_WhenCarPartNotUpdated_ReturnsServerError()
    {
        // arrange
        Guid carPartId = Guid.NewGuid();
        var request = new CarPartRequest(
            string.Empty,
            180_000,
            120_000,
            new DateOnly(2020, 5, 5),
            120
        );

        _mockCarPartsService.Setup(service => service.UpdateAsync(It.IsAny<UpdateCarPartDto>()))
            .ReturnsAsync(false);

        // act
        var result = await _controller.Update(carPartId, request);

        // assert
        var serverErrorResponse = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(500, serverErrorResponse.StatusCode);
    }

    [Fact]
    public async Task Update_WhenCarPartNotFound_ThrowsKeyNotFound()
    {
        // arrange
        Guid carPartId = Guid.NewGuid();
        var request = new CarPartRequest(
            string.Empty,
            180_000,
            120_000,
            new DateOnly(2020, 5, 5),
            120
        );

        _mockCarPartsService.Setup(service => service.UpdateAsync(It.IsAny<UpdateCarPartDto>()))
        .ThrowsAsync(new KeyNotFoundException("not found"));

        // act
        var result = _controller.Update(carPartId, request);

        // assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => result);
    }
}
