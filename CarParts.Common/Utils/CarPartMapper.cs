using System;
using System.Collections.Generic;
using System.Text;
using CarParts.Common.DTOs;
using CarParts.Common.Entities;
using CarParts.Common.Models;

namespace CarParts.Common.Utils
{
    public static class CarPartMapper
    {
        public static CarPart ToEntity(this CreateCarPartDto createCarPartDto)
        {
            return new CarPart
            {
                Name = createCarPartDto.Name,
                InstalledAtKm = createCarPartDto.InstalledAtKm,
                MaxKmLifetime = createCarPartDto.MaxKmLifetime,
                InstalledAtDate = createCarPartDto.InstalledAtDate,
                MaxMonthsLifetime = createCarPartDto.MaxMonthsLifetime,
            };
        }

        public static CarPartDto ToDto(this CarPart carPart)
        {
            return new CarPartDto(
                carPart.Id,
                carPart.Name,
                carPart.InstalledAtKm,
                carPart.MaxKmLifetime,
                carPart.InstalledAtDate,
                carPart.MaxMonthsLifetime
            );
        }

        public static CarPartResponse ToResponse(this CarPartDto carPartDto)
        {
            return new CarPartResponse(
                carPartDto.Id,
                carPartDto.Name,
                carPartDto.InstalledAtKm,
                carPartDto.MaxKmLifetime,
                carPartDto.InstalledAtDate,
                carPartDto.MaxMonthsLifetime
            );
        }

        public static CreateCarPartDto ToCreateDto(this CarPartRequest carPartRequest, Guid userId)
        {
            return new CreateCarPartDto(
                carPartRequest.Name,
                carPartRequest.InstalledAtKm,
                carPartRequest.MaxKmLifetime,
                carPartRequest.InstalledAtDate,
                carPartRequest.MaxMonthsLifetime,
                userId
            );
        }

        public static CarPart ToEntity(this UpdateCarPartDto updateCarPartDto)
        {
            return new CarPart
            {
                Id = updateCarPartDto.Id,
                Name = updateCarPartDto.Name,
                InstalledAtKm = updateCarPartDto.InstalledAtKm,
                MaxKmLifetime = updateCarPartDto.MaxKmLifetime,
                InstalledAtDate = updateCarPartDto.InstalledAtDate,
                MaxMonthsLifetime = updateCarPartDto.MaxMonthsLifetime
            };
        }

        public static UpdateCarPartDto ToUpdateDto(this CarPartRequest carPartRequest, Guid id)
        {
            return new UpdateCarPartDto(
                id,
                carPartRequest.Name,
                carPartRequest.InstalledAtKm,
                carPartRequest.MaxKmLifetime,
                carPartRequest.InstalledAtDate,
                carPartRequest.MaxMonthsLifetime
            );
        }

        public static RegistrationResponse ToModel(this User user)
        {
            return new RegistrationResponse(
                user.Id,
                user.Username,
                user.Email
            );
        }
    }
}
