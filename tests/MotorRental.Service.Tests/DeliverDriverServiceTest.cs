using Microsoft.AspNetCore.Http;
using Moq;
using MotorRental.Application.Common;
using MotorRental.Application.Services;
using MotorRental.Domain.Constants;
using MotorRental.Domain.Dtos;
using MotorRental.Domain.Entities;
using MotorRental.Domain.Interfaces;

namespace MotorRental.Service.Tests
{
    public class DeliverDriverServiceTest
    {
        private readonly Mock<IAwsS3Service> _awsS3ServiceMock;
        private readonly Mock<IDeliverDriverRepository> _driverRepositoryMock;
        private readonly Mock<IFormFile> _formFileMock;
        public DeliverDriverServiceTest()
        {
            _driverRepositoryMock = new Mock<IDeliverDriverRepository>();
            _awsS3ServiceMock = new Mock<IAwsS3Service>();
            _formFileMock = new Mock<IFormFile>();
        }

        [Fact]
        public async Task UploadLicenseDriverPhotoAsync_Should_Return_ApiResponse_Ok()
        {
            var uploadLicenseDriverPhotoDto = new UploadLicenseDriverPhotoDto
            {
                DeliverDriverId = 1,
                LicenseDriverPhoto = _formFileMock.Object
            };
            var deliverDriver = new DeliverDriver { Id = 1, Email = "flucasrodrigues@hotmail.com" };

            _driverRepositoryMock.Setup(a => a.GetByIdAsync(uploadLicenseDriverPhotoDto.DeliverDriverId)).ReturnsAsync(deliverDriver);

            var service = new DeliverDriverService(_awsS3ServiceMock.Object, _driverRepositoryMock.Object);

            var result = await service.UploadLicenseDriverPhotoAsync(uploadLicenseDriverPhotoDto);

            var okResult = Assert.IsType<ApiResponse>(result);
            Assert.True(okResult.Success);
            Assert.Equal(ErrorMessagesConstants.SUCCESS_OK, okResult.Message);
            Assert.Null(okResult.Data);
        }
    }
}