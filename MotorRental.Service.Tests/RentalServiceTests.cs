using Moq;
using MotorRental.Application.Services;
using MotorRental.Domain.Constants;
using MotorRental.Domain.Dtos;
using MotorRental.Domain.Entities;
using MotorRental.Domain.Enums;
using MotorRental.Domain.Interfaces;

namespace MotorRental.Application.Tests
{
    public class RentalServiceTests
    {
        private readonly Mock<IMotorcycleRepository> _motorcycleRepositoryMock;
        private readonly Mock<IDeliverDriverRepository> _deliverDriverRepositoryMock;
        private readonly Mock<IPlanRepository> _planRepositoryMock;
        private readonly Mock<IRentalRepository> _rentalRepositoryMock;
        private readonly Mock<IMessagingService> _messagingServiceMock;
        private readonly RentalService _rentalService;

        public RentalServiceTests()
        {
            _motorcycleRepositoryMock = new Mock<IMotorcycleRepository>();
            _deliverDriverRepositoryMock = new Mock<IDeliverDriverRepository>();
            _planRepositoryMock = new Mock<IPlanRepository>();
            _rentalRepositoryMock = new Mock<IRentalRepository>();
            _messagingServiceMock = new Mock<IMessagingService>();
            _rentalService = new RentalService(_rentalRepositoryMock.Object, _deliverDriverRepositoryMock.Object, _planRepositoryMock.Object, _motorcycleRepositoryMock.Object);
        }

        [Theory]
        [InlineData(7, 30)]
        [InlineData(15, 28)]
        [InlineData(30, 22)]
        [InlineData(45, 20)]
        [InlineData(50, 18)]
        public async Task RentAMotorcycle_Should_Return_Ok(int numberOfDaysPLan, int dailyPricePlan)
        {
            var rentDto = new RentAMotorcycleDto { MotorcycleId = 1, DeliverDriverId = 1, PlanId = 1 };
            var motorcycle = new Motorcycle { Id = 1 };
            var deliverDriver = new DeliverDriver { Id = 1, LicenseDriverType = LicenseDriverTypeEnum.A };
            var plan = new Plan { NumberOfDays = numberOfDaysPLan, DailyPrice = dailyPricePlan };

            _motorcycleRepositoryMock.Setup(r => r.GetByIdAsync(rentDto.MotorcycleId)).ReturnsAsync(motorcycle);
            _deliverDriverRepositoryMock.Setup(r => r.GetByIdAsync(rentDto.DeliverDriverId)).ReturnsAsync(deliverDriver);
            _planRepositoryMock.Setup(r => r.GetByIdAsync(rentDto.PlanId)).ReturnsAsync(plan);

            var result = await _rentalService.RentAMotorcycle(rentDto);

            Assert.True(result.Success);
            Assert.Equal(plan.NumberOfDays * plan.DailyPrice, ((Rental?)result.Data).ExpectedPrice);
            Assert.Equal(rentDto.StartDate.AddDays(plan.NumberOfDays).Date, ((Rental?)result.Data).ExpectedEndDate.Value.Date);
            _rentalRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Rental?>(), null), Times.Once);
        }

        [Theory]
        [InlineData(0, 1, 1)]
        [InlineData(1, 0, 1)]
        [InlineData(1, 1, 0)]
        public async Task RentAMotorcycle_Should_Return_BadRequest_When_Some_Entity_Not_Found(int motorcycleId, int deliverDriverId, int planId)
        {
            // Arrange
            var rentDto = new RentAMotorcycleDto { MotorcycleId = motorcycleId, DeliverDriverId = deliverDriverId, PlanId = planId };
            _motorcycleRepositoryMock.Setup(r => r.GetByIdAsync(rentDto.MotorcycleId)).ReturnsAsync((Motorcycle?)null);

            // Act
            var result = await _rentalService.RentAMotorcycle(rentDto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains(ErrorMessagesConstants.NO_MOTORCYCLE_AVAILABLE, result.Errors);
        }

        [Fact]
        public async Task RentAMotorcycle_Should_Return_BadRequest_When_License_Isnt_A()
        {
            var rentDto = new RentAMotorcycleDto { MotorcycleId = 1, DeliverDriverId = 1, PlanId = 1 };
            var motorcycle = new Motorcycle { Id = 1 };
            var deliverDriver = new DeliverDriver { Id = 1, LicenseDriverType = LicenseDriverTypeEnum.B };
            var plan = new Plan { NumberOfDays = 7, DailyPrice = 30 };

            _motorcycleRepositoryMock.Setup(r => r.GetByIdAsync(rentDto.MotorcycleId)).ReturnsAsync(motorcycle);
            _deliverDriverRepositoryMock.Setup(r => r.GetByIdAsync(rentDto.DeliverDriverId)).ReturnsAsync(deliverDriver);
            _planRepositoryMock.Setup(r => r.GetByIdAsync(rentDto.PlanId)).ReturnsAsync(plan);

            var result = await _rentalService.RentAMotorcycle(rentDto);

            Assert.False(result.Success);
            Assert.Contains(ErrorMessagesConstants.DELIVER_DRIVER_NOT_QUALIFIED, result.Errors);
        }

        [Theory]
        [InlineData(7, 30, "2024-08-08")]
        [InlineData(15, 28, "2024-08-16")]
        [InlineData(30, 22, "2024-08-31")]
        [InlineData(45, 20, "2024-09-15")]
        [InlineData(50, 18, "2024-09-20")]
        public async Task InformEndDateRental_Should_Return_Right_Values_EndDate_And_Price(int numberOfDaysPLan, int dailyPricePlan, DateTime endDate)
        {
            var rental = new Rental { Id = 1, ExpectedEndDate = endDate, Plan = new Plan { DailyPrice = dailyPricePlan, NumberOfDays = numberOfDaysPLan }, StartDate = new DateTime(2024, 08, 01) };
            var informDto = new InformEndDateRentalDto { RentalId = 1, EndDate = endDate };

            _rentalRepositoryMock.Setup(r => r.GetByIdAsync(informDto.RentalId)).ReturnsAsync(rental);

            var result = await _rentalService.InformEndDateRental(informDto);

            Assert.True(result.Success);
            Assert.Equal(rental.Plan.NumberOfDays * rental.Plan.DailyPrice, ((Rental?)result.Data).Price);
            Assert.Equal(rental.StartDate.AddDays(rental.Plan.NumberOfDays).Date, ((Rental?)result.Data).EndDate.Value.Date);
            _rentalRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Rental?>(), null), Times.Once);
        }

        [Theory]
        [InlineData(7, 30, "2024-08-10", 370)]
        [InlineData(15, 28, "2024-08-18", 576)]
        [InlineData(30, 22, "2024-09-02", 804)]
        [InlineData(45, 20, "2024-09-17", 1040)]
        [InlineData(50, 18, "2024-09-22", 1036)]
        public async Task InformEndDateRental_Should_Return_Right_Values_ExpectedDate_And_Price_When_Is_Late(int numberOfDaysPLan, int dailyPricePlan, DateTime endDate, decimal expectedPrice)
        {
            var informDto = new InformEndDateRentalDto { RentalId = 1, EndDate = endDate };
            var rental = new Rental
            {
                Id = 1,
                ExpectedEndDate = new DateTime(2024, 08, 01).AddDays(numberOfDaysPLan),
                Plan = new Plan { DailyPrice = dailyPricePlan, NumberOfDays = numberOfDaysPLan },
                StartDate = new DateTime(2024, 08, 01)
            };

            _rentalRepositoryMock.Setup(r => r.GetByIdAsync(informDto.RentalId)).ReturnsAsync(rental);

            var result = await _rentalService.InformEndDateRental(informDto);

            Assert.True(result.Success);
            Assert.Equal(expectedPrice, ((Rental?)result.Data).Price);
            Assert.Equal(endDate, ((Rental?)result.Data).EndDate);
            _rentalRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Rental?>(), null), Times.Once);
        }

        [Theory]
        [InlineData(7, 30, 222)]
        [InlineData(15, 28, 442.4)]
        [InlineData(30, 22, 677.6)]
        [InlineData(45, 20, 916)]
        [InlineData(50, 18, 914.4)]
        public async Task InformEndDateRental_Should_Return_Right_Values_ExpectedDate_And_Price_When_Is_Early(int numberOfDaysPLan, int dailyPricePlan, decimal expectedPrice)
        {
            var informDto = new InformEndDateRentalDto { RentalId = 1, EndDate = DateTime.Now };
            var rental = new Rental
            {
                Id = 1,
                ExpectedEndDate = DateTime.Now.AddDays(-(numberOfDaysPLan - 2)).AddDays(numberOfDaysPLan),
                Plan = new Plan { DailyPrice = dailyPricePlan, NumberOfDays = numberOfDaysPLan },
                StartDate = DateTime.Now.AddDays(-(numberOfDaysPLan - 2))
            };

            _rentalRepositoryMock.Setup(r => r.GetByIdAsync(informDto.RentalId)).ReturnsAsync(rental);

            var result = await _rentalService.InformEndDateRental(informDto);

            Assert.True(result.Success);
            Assert.Equal(expectedPrice, ((Rental?)result.Data).Price);
            Assert.Equal(rental.ExpectedEndDate, ((Rental?)result.Data).ExpectedEndDate);
            Assert.Equal(informDto.EndDate, ((Rental?)result.Data).EndDate);
            _rentalRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Rental>(), null), Times.Once);
        }

        [Fact]
        public async Task InformEndDateRental_Should_Return_BadRequest_When_Rental_NotFound()
        {
            var informDto = new InformEndDateRentalDto { RentalId = 1, EndDate = DateTime.Now.AddDays(1) };
            _rentalRepositoryMock.Setup(r => r.GetByIdAsync(informDto.RentalId)).ReturnsAsync((Rental?)null);

            var result = await _rentalService.InformEndDateRental(informDto);

            Assert.False(result.Success);
            Assert.Contains(ErrorMessagesConstants.RENTAL_DOESNT_EXIST, result.Errors);
        }
    }
}