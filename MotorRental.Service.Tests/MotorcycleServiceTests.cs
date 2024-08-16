using Moq;
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
        public void Get_Should_Return_Filtered_Motorcycles()
        {
            // Arrange
            var motorcycles = new List<Motorcycle>
            {
                new Motorcycle { Id = 1, LicensePlate = "ABC123", Model = "Model1", Year = 2020 },
                new Motorcycle { Id = 2, LicensePlate = "DEF456", Model = "Model2", Year = 2021 }
            }.AsQueryable();

            _motorcyleRepositoryMock.Setup(r => r.GetAll()).Returns(motorcycles);

            var service = new MotorcycleService(_motorcyleRepositoryMock.Object, null);

            var filter = new GetMotorcyclesFilterDto { LicensePlate = "ABC" };

            // Act
            var result = service.Get(filter);

            // Assert
            Assert.Single(result);
            Assert.Equal("ABC123", result.First().LicensePlate);
        }

        [Fact]
        public async Task Get_Should_Return_All_Motorcycles()
        {
            var motorcycles = new List<Motorcycle>
                {
                    new Motorcycle
                    {
                        Id = 1,
                        LicensePlate = "ABC1234",
                        Model = "TIGER",
                        Year = 2024
                    },
                    new Motorcycle
                    {
                        Id = 2,
                        LicensePlate = "DEF1235",
                        Model = "CG TITAN",
                        Year = 2023
                    },
                };
            _motorcyleRepositoryMock.Setup(a => a.GetAll()).Returns(motorcycles.AsQueryable);

            var service = new MotorcycleService(_motorcyleRepositoryMock.Object, _messagingServiceMock.Object);

            var result = service.GetAll().ToList();
            Assert.IsType<List<Motorcycle>>(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void Get_Should_Return_Empty_When_No_Motorcycles_Match_Filter()
        {
            var motorcycles = new List<Motorcycle>().AsQueryable();

            var filter = new GetMotorcyclesFilterDto { LicensePlate = "ABC" };

            _motorcyleRepositoryMock.Setup(r => r.GetAll()).Returns(motorcycles);

            var service = new MotorcycleService(_motorcyleRepositoryMock.Object, null);

            var result = service.Get(filter);

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
                Year = motorcycleDto.Year
            };

            _motorcyleRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Motorcycle>(), null)).ReturnsAsync(addedMotorcycle);

            await service.AddMotorcycle(motorcycleDto);

            _motorcyleRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Motorcycle>(), null), Times.Once);
            _messagingServiceMock.Verify(m => m.SendMessage(RabbitMqConstants.MOTORCYCLE_NOTIFICATION_QUEUE_NAME, It.Is<string>(s => s.Contains("ABC123"))), Times.Once);
        }

        [Fact]
        public async Task AddMotorcycle_Should_Throw_Exception_When_Repository_Fails()
        {
            var motorcycleDto = new MotorcycleDto
            {
                LicensePlate = "ABC123",
                Model = "Model1",
                Year = 2020
            };

            var service = new MotorcycleService(_motorcyleRepositoryMock.Object, _messagingServiceMock.Object);

            _motorcyleRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Motorcycle>(), null)).ThrowsAsync(new EntityException());

            await Assert.ThrowsAsync<EntityException>(() => service.AddMotorcycle(motorcycleDto));
        }

        [Fact]
        public async Task AddMotorcycle_Should_Throw_Exception_When_Messaging_Service_Fails()
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
                Year = motorcycleDto.Year
            };

            _motorcyleRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Motorcycle>(), null)).ReturnsAsync(addedMotorcycle);
            _messagingServiceMock.Setup(m => m.SendMessage(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());

            var exception = await Assert.ThrowsAsync<Exception>(() => service.AddMotorcycle(motorcycleDto));
        }

        [Fact]
        public async Task UpdateLicensePlate_ShouldUpdateLicensePlate_When_Motorcycle_Exists()
        {
            var updateDto = new UpdateLicensePlateDto
            {
                MotorcycleId = 1,
                LicensePlate = "ABC6789"
            };

            var existingMotorcycle = new Motorcycle
            {
                Id = 1,
                LicensePlate = "ABC1234",
                Model = "Model1",
                Year = 2020
            };

            _motorcyleRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(existingMotorcycle);
            _motorcyleRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Motorcycle>(), null));

            var service = new MotorcycleService(_motorcyleRepositoryMock.Object, null);

            await service.UpdateLicensePlate(updateDto);

            _motorcyleRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _motorcyleRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Motorcycle>(m => m.LicensePlate == "ABC6789"), null), Times.Once);
        }

        [Fact]
        public async Task UpdateLicensePlate_Should_Do_Nothing_When_Motorcycle_Does_Not_Exist()
        {
            var updateDto = new UpdateLicensePlateDto
            {
                MotorcycleId = 1,
                LicensePlate = "XYZ789"
            };

            _motorcyleRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Motorcycle?)null);

            var service = new MotorcycleService(_motorcyleRepositoryMock.Object, null);

            await service.UpdateLicensePlate(updateDto);

            _motorcyleRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _motorcyleRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Motorcycle>(), null), Times.Never);
        }

        [Fact]
        public async Task UpdateLicensePlate_Should_Throw_Exception_When_Repository_Fails()
        {
            var updateDto = new UpdateLicensePlateDto
            {
                MotorcycleId = 1,
                LicensePlate = "XYZ789"
            };

            var existingMotorcycle = new Motorcycle
            {
                Id = 1,
                LicensePlate = "ABC123",
                Model = "Model1",
                Year = 2020
            };

            _motorcyleRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(existingMotorcycle);
            _motorcyleRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Motorcycle>(), null)).ThrowsAsync(new Exception());

            var service = new MotorcycleService(_motorcyleRepositoryMock.Object, null);

            await Assert.ThrowsAsync<Exception>(() => service.UpdateLicensePlate(updateDto));
        }
    }
}
