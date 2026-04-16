using CarParts.API.Middleware.Authentication;
using CarParts.Common.Entities;
using CarParts.Common.Models;
using CarParts.Common.Utils;
using CarParts.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarParts.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarPartsController : ControllerBase
    {
        private readonly ICarPartsService _carPartsService;

        public CarPartsController(ICarPartsService carPartsService)
        {
            _carPartsService = carPartsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var carParts = await _carPartsService.GetAllAsync();
            if (!carParts.Any())
            {
                return NoContent();
            }
            return Ok(carParts.Select(part => part.ToResponse()));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("id cant be empty");
            }

            var carPart = await _carPartsService.GetByIdAsync(id);
            if (carPart == null)
            {
                return NotFound($"car with given id doesnt exist");
            }

            return Ok(carPart.ToResponse());
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CarPartRequest carPartRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var fakeUserId = Guid.NewGuid();

            var createdCarPartDto = await _carPartsService.AddAsync(carPartRequest.ToCreateDto(fakeUserId));
            var createdCarPartResponse = createdCarPartDto.ToResponse();

            return CreatedAtAction(
                nameof(GetById),
                new { Id = createdCarPartResponse.Id },
                createdCarPartResponse
            );
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Remove([FromRoute] Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("id cant be empty");
            }

            var removedCarPart = await _carPartsService.RemoveAsync(id);
            if (removedCarPart)
            {
                return Ok();
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] CarPartRequest carPartRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id == Guid.Empty)
            {
                return BadRequest("id cant be empty");
            }

            var isSuccess = await _carPartsService.UpdateAsync(carPartRequest.ToUpdateDto(id));
            if (!isSuccess)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok();
        }
    }
}
