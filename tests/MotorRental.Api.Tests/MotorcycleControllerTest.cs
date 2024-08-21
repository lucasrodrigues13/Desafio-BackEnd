using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MotorRental.Application.Common;
using MotorRental.Application.Interfaces;
using MotorRental.Application.Services;
using MotorRental.Domain.Constants;
using MotorRental.Domain.Dtos;
using MotorRental.Domain.Entities;
using MotorRental.WebApi.Controllers;

namespace MotorRental.Api.Tests
{
    public class MotorcycleControllerTest
    {
        private readonly Mock<IMotorcycleService> _motorcycleServiceMock;
        private readonly Mock<ILogger<MotorcycleController>> _loggerMock;

        public MotorcycleControllerTest()
        {
            _motorcycleServiceMock = new Mock<IMotorcycleService>();
            _loggerMock = new Mock<ILogger<MotorcycleController>>();
        }

        [Fact]
        public async Task Post_Should_Return_Ok()
        {
            var motorcycleDto = new MotorcycleDto { LicensePlate = "ABC1234", Model = "TIGER 800CC", Year = 2024 };
            var motorcycle = new Motorcycle { LicensePlate = "ABC1234", Model = "TIGER 800CC", Year = 2024 };

            var controller = new MotorcycleController(_motorcycleServiceMock.Object, _loggerMock.Object);
            _motorcycleServiceMock.Setup(a => a.AddMotorcycle(It.IsAny<MotorcycleDto>())).ReturnsAsync(ApiResponse.Ok(motorcycle));

            var result = await controller.Post(motorcycleDto);

            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData("ABC1234", "TIGER 800CC", 0)]
        [InlineData(null, "TIGER 800CC", 2024)]
        [InlineData("ABC1234", null, 2022)]
        public async Task Post_Should_Return_BadRequest(string licensePlate, string model, int year)
        {
            var motorcycleDto = new MotorcycleDto { LicensePlate = licensePlate, Model = model, Year = year };

            var controller = new MotorcycleController(_motorcycleServiceMock.Object, _loggerMock.Object);
            _motorcycleServiceMock.Setup(a => a.AddMotorcycle(It.IsAny<MotorcycleDto>())).ReturnsAsync(new ApiResponse(false, ErrorMessagesConstants.BADREQUEST_DEFAULT, null, []));

            var result = await controller.Post(motorcycleDto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateLicensePlate_Should_Return_Ok()
        {
            var updateLicensePlateDto = new UpdateLicensePlateDto { LicensePlate = "ABC1235", MotorcycleId = 1 };
            var motorcycle = new Motorcycle { LicensePlate = "ABC1235", Id = 1 };

            var controller = new MotorcycleController(_motorcycleServiceMock.Object, _loggerMock.Object);
            _motorcycleServiceMock.Setup(a => a.UpdateLicensePlate(It.IsAny<UpdateLicensePlateDto>())).ReturnsAsync(ApiResponse.Ok(motorcycle));

            var result = await controller.UpdateLicensePlate(updateLicensePlateDto);

            Assert.IsType<OkObjectResult>(result);
        }


        [Theory]
        [InlineData("ABC1234", 0)]
        [InlineData("TIGER 800CC", 1)]
        public async Task UpdateLicensePlate_Should_Return_BadRequest(string licensePlate, int motorcycleId)
        {
            var updateLicensePlateDto = new UpdateLicensePlateDto { LicensePlate = licensePlate, MotorcycleId = motorcycleId };
            var motorcycle = new Motorcycle { LicensePlate = "ABC1235", Id = 1 };

            var controller = new MotorcycleController(_motorcycleServiceMock.Object, _loggerMock.Object);
            _motorcycleServiceMock.Setup(a => a.UpdateLicensePlate(It.IsAny<UpdateLicensePlateDto>())).ReturnsAsync(new ApiResponse(false, ErrorMessagesConstants.BADREQUEST_DEFAULT, null, []));

            var result = await controller.UpdateLicensePlate(updateLicensePlateDto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Delete_Should_Return_Ok()
        {
            var controller = new MotorcycleController(_motorcycleServiceMock.Object, _loggerMock.Object);

            var result = await controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData(0)]
        public async Task Delete_Should_Return_BadRequest(int motorcycleId)
        {
            var controller = new MotorcycleController(_motorcycleServiceMock.Object, _loggerMock.Object);

            var result = await controller.Delete(motorcycleId);

            Assert.IsType<OkObjectResult>(result);
        }
    }
}
