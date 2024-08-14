using Microsoft.AspNetCore.Mvc;
using Moq;
using MotorRental.Application.Common;
using MotorRental.Application.Interfaces;
using MotorRental.Domain.Entities;
using MotorRental.WebApi.Controllers;

namespace MotorRental.Api.Tests
{
    public class RentalControllerTest
    {
        private readonly Mock<IRentalService> _rentalServiceMock;

        public RentalControllerTest()
        {
            _rentalServiceMock = new Mock<IRentalService>();
        }

        [Fact]
        public async Task Get_Should_Return_Ok()
        {
            var rentals = new List<Rental>
                {
                    new Rental
                    {
                        Id = 1,
                        DeliverDriverId = 1,
                        MotorcycleId = 1,
                        PlanId = 1,
                        StartDate = DateTime.Now,
                        ExpectedEndDate = DateTime.Now.AddDays(7),
                        ExpectedPrice = 210,
                    },
                    new Rental
                    {
                        Id = 2,
                        DeliverDriverId = 2,
                        MotorcycleId = 2,
                        PlanId = 2,
                        StartDate = DateTime.Now,
                        ExpectedEndDate = DateTime.Now.AddDays(15),
                        ExpectedPrice = 420,
                    },
                };
            _rentalServiceMock.Setup(a => a.GetAll()).Returns(rentals.AsQueryable());

            var controller = new RentalController(_rentalServiceMock.Object);

            var result = controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultValueApiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            var resultData = Assert.IsType<List<Rental>>(resultValueApiResponse.Data);
            Assert.Equal(2, resultData.Count);
        }

        [Fact]
        public void Get_Should_Return_NoContent()
        {
            _rentalServiceMock.Setup(service => service.GetAll()).Returns(new List<Rental>().AsQueryable());

            var controller = new RentalController(_rentalServiceMock.Object);
            var result = controller.Get();

            Assert.IsType<NoContentResult>(result);
        }
    }
}
