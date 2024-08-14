using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MotorRental.Application.Common;
using MotorRental.Application.Interfaces;
using MotorRental.Domain.Constants;
using MotorRental.Domain.Dtos;
using MotorRental.WebApi.Controllers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MotorRental.Api.Tests
{
    public class DeliverDriverControllerTest
    {
        private readonly Mock<IDeliverDriverService> _driverServiceMock;
        private readonly Mock<IRentalService> _rentalServiceMock;
        private readonly Mock<IFormFile> _formFileMock;

        public DeliverDriverControllerTest()
        {
            _driverServiceMock = new Mock<IDeliverDriverService>();
            _rentalServiceMock = new Mock<IRentalService>();
            _formFileMock = new Mock<IFormFile>();
        }

        [Fact]
        public async Task UploadLicenseDriverPhoto_Should_Return_Ok()
        {
            var uploadLicenseDriverPhotoDto = new UploadLicenseDriverPhotoDto
            {
                DeliverDriverId = 1,
                LicenseDriverPhoto = _formFileMock.Object
            };
            _driverServiceMock.Setup(a => a.UploadLicenseDriverPhotoAsync(uploadLicenseDriverPhotoDto, "test@email.com.br")).ReturnsAsync(ApiResponse.Ok());

            var controller = new DeliverDriverController(_driverServiceMock.Object, _rentalServiceMock.Object);

            var result = await controller.UploadLicenseDriverPhoto(uploadLicenseDriverPhotoDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<ApiResponse>(okResult.Value);
        }


        [Fact]
        public async Task UploadLicenseDriverPhoto_Should_Return_BadRequest()
        {
            var uploadLicenseDriverPhotoDto = new UploadLicenseDriverPhotoDto
            {
                DeliverDriverId = 1,
                LicenseDriverPhoto = null
            };
            _driverServiceMock.Setup(a => a.UploadLicenseDriverPhotoAsync(uploadLicenseDriverPhotoDto, "test@email.com.br"))
                .ReturnsAsync(new ApiResponse(false, ErrorMessagesConstants.BADREQUEST_DEFAULT, null, []));

            var controller = new DeliverDriverController(_driverServiceMock.Object, _rentalServiceMock.Object);

            var result = await controller.UploadLicenseDriverPhoto(uploadLicenseDriverPhotoDto);

            var okResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<ApiResponse>(okResult.Value);
        }

        [Fact]
        public async Task RentAMotorcycle_Sould_Return_Ok()
        {
            var rentAMotorcycleDto = new RentAMotorcycleDto { DeliverDriverId = 1, MotorcycleId = 1, PlanId = 1 };
            _rentalServiceMock.Setup(a => a.RentAMotorcycle(rentAMotorcycleDto)).ReturnsAsync(ApiResponse.Ok());

            var controller = new DeliverDriverController(_driverServiceMock.Object, _rentalServiceMock.Object);

            var result = await controller.RentAMotorcycle(rentAMotorcycleDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<ApiResponse>(okResult.Value);
        }

        [Fact]
        public async Task InformEndDateRental_Sould_Return_Ok()
        {
            var informEndDateRentalDto = new InformEndDateRentalDto { RentalId = 1, EndDate = DateTime.Now.AddDays(4) };
            _rentalServiceMock.Setup(a => a.InformEndDateRental(informEndDateRentalDto)).ReturnsAsync(ApiResponse.Ok());

            var controller = new DeliverDriverController(_driverServiceMock.Object, _rentalServiceMock.Object);

            var result = await controller.InformEndDateRental(informEndDateRentalDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<ApiResponse>(okResult.Value);
        }
    }
}
