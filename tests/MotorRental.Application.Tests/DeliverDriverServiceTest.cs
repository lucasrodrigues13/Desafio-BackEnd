using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using MotorRental.Application.Common;
using MotorRental.Application.Services;
using MotorRental.Domain.Constants;
using MotorRental.Domain.Dtos;
using MotorRental.Domain.Entities;
using MotorRental.Domain.Interfaces;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace MotorRental.Application.Tests
{
    public class DeliverDriverServiceTest
    {
        private readonly Mock<IAwsS3Service> _awsS3ServiceMock;
        private readonly Mock<IDeliverDriverRepository> _driverRepositoryMock;
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        public DeliverDriverServiceTest()
        {
            _driverRepositoryMock = new Mock<IDeliverDriverRepository>();
            _awsS3ServiceMock = new Mock<IAwsS3Service>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>();
        }

        [Fact]
        public async Task UploadLicenseDriverPhotoAsync_Should_Return_ApiResponse_Ok()
        {
            // Arrange
            var fileName = "test.png";
            var contentType = "image/jpeg";

            // Create a sample image in memory (100x100 white square)
            byte[] imageBytes;
            using (var bitmap = new Bitmap(100, 100))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(Color.White);
                }
                using (var ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Jpeg);
                    imageBytes = ms.ToArray();
                }
            }

            var formFile = CreateMockFormFile(fileName, contentType, imageBytes);

            var uploadLicenseDriverPhotoDto = new UploadLicenseDriverPhotoDto
            {
                DeliverDriverId = 1,
                LicenseDriverPhoto = formFile
            };
            var deliverDriver = new DeliverDriver { Id = 1, Email = "flucasrodrigues@hotmail.com" };

            _driverRepositoryMock.Setup(a => a.GetByIdAsync(uploadLicenseDriverPhotoDto.DeliverDriverId)).ReturnsAsync(deliverDriver);

            var service = new DeliverDriverService(_awsS3ServiceMock.Object, _driverRepositoryMock.Object, _userManagerMock.Object);

            var result = await service.UploadLicenseDriverPhotoAsync(uploadLicenseDriverPhotoDto);

            var okResult = Assert.IsType<ApiResponse>(result);
            Assert.True(okResult.Success);
            Assert.Equal(ErrorMessagesConstants.SUCCESS_OK, okResult.Message);
            Assert.Null(okResult.Data);
        }

        private IFormFile CreateMockFormFile(string fileName, string contentType, byte[] content)
        {
            var fileMock = new Mock<IFormFile>();
            var stream = new MemoryStream(content);

            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.ContentType).Returns(contentType);
            fileMock.Setup(f => f.ContentDisposition).Returns($"form-data; name=\"{fileName}\"; filename=\"{fileName}\"");

            return fileMock.Object;
        }
    }
}