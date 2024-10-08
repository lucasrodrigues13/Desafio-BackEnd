﻿using Moq;
using MotorRental.Application.Services;
using MotorRental.Domain.Constants;
using MotorRental.Domain.Dtos;
using MotorRental.Domain.Entities;
using MotorRental.Domain.Interfaces;
using System.Data.Entity.Core;

namespace MotorRental.Application.Tests
{
    public class MotorcycleServiceTests
    {
        private readonly Mock<IMotorcycleRepository> _motorcyleRepositoryMock;
        private readonly Mock<IMessagingService> _messagingServiceMock;

        public MotorcycleServiceTests()
        {
            _messagingServiceMock = new Mock<IMessagingService>();
            _motorcyleRepositoryMock = new Mock<IMotorcycleRepository>();
        }

        [Fact]
        public async Task Get_Should_Return_Filtered_Motorcycles()
        {
            var motorcycles = new List<MotorcycleDto>
            {
                new MotorcycleDto { Id = 1, LicensePlate = "ABC1234", Model = "Model1", Year = 2020 },
                new MotorcycleDto { Id = 2, LicensePlate = "ABC4564", Model = "Model2", Year = 2024 }
            };
            var filter = new GetMotorcyclesFilterDto { LicensePlate = "ABC" };

            _motorcyleRepositoryMock.Setup(r => r.GetByLicensePlate(filter.LicensePlate)).ReturnsAsync(motorcycles);

            var service = new MotorcycleService(_motorcyleRepositoryMock.Object, null);


            var result = await service.Get(filter);

            Assert.IsType<List<MotorcycleDto>>(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task Get_Should_Return_All_Motorcycles()
        {
            var motorcycles = new List<MotorcycleDto>
            {
                new MotorcycleDto { Id = 1, LicensePlate = "ABC1234", Model = "Model1", Year = 2020 },
                new MotorcycleDto { Id = 2, LicensePlate = "ABC4564", Model = "Model2", Year = 2024 }
            };
            var filter = new GetMotorcyclesFilterDto { LicensePlate = string.Empty };

            _motorcyleRepositoryMock.Setup(r => r.GetByLicensePlate(filter.LicensePlate)).ReturnsAsync(motorcycles);

            var service = new MotorcycleService(_motorcyleRepositoryMock.Object, _messagingServiceMock.Object);

            var result = await service.Get(filter);
            Assert.IsType<List<MotorcycleDto>>(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task Get_Should_Return_Empty_When_No_Motorcycles_Match_Filter()
        {
            var motorcycles = new List<MotorcycleDto>();

            var filter = new GetMotorcyclesFilterDto { LicensePlate = "ABC" };

            _motorcyleRepositoryMock.Setup(r => r.GetByLicensePlate(filter.LicensePlate)).ReturnsAsync(motorcycles);

            var service = new MotorcycleService(_motorcyleRepositoryMock.Object, null);

            var result = await service.Get(filter);

            Assert.Empty(result);
        }

        [Fact]
        public async Task AddMotorcycle_ShouldAddMotorcycle_AndSendMessage()
        {
            var motorcycleDto = new MotorcycleDto
            {
                LicensePlate = "ABC123",
                Model = "Model1",
                Year = 2020
            };

            var service = new MotorcycleService(_motorcyleRepositoryMock.Object, _messagingServiceMock.Object);

            var addedMotorcycle = new Motorcycle
            {
                Id = 1,
                LicensePlate = motorcycleDto.LicensePlate,
                Model = motorcycleDto.Model,
                Year = motorcycleDto.Year.Value
            };

            _motorcyleRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Motorcycle>(), null)).ReturnsAsync(addedMotorcycle);
            _motorcyleRepositoryMock.Setup(r => r.GetByLicensePlate(It.IsAny<string>())).ReturnsAsync(new List<MotorcycleDto>());

            await service.AddMotorcycle(motorcycleDto);

            _motorcyleRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Motorcycle>(), null), Times.Once);
            _messagingServiceMock.Verify(m => m.SendMessage(RabbitMqConstants.MOTORCYCLE_NOTIFICATION_QUEUE_NAME, It.Is<string>(s => s.Contains("ABC123"))), Times.Once);
        }
    }
}
