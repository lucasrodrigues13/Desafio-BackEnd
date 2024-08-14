using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using MotorRental.Application.Interfaces;
using MotorRental.Domain.Constants;
using MotorRental.Domain.Dtos;
using MotorRental.Domain.Entities;
using MotorRental.WebApi.Controllers;

namespace MotorRental.Api.Tests
{
    public class AccountControllerTest
    {
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<SignInManager<IdentityUser>> _signInManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IDeliverDriverService> _deliverDriverServiceMock;
        public AccountControllerTest()
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);

            var contextAccessor = new Mock<IHttpContextAccessor>();
            var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
            _signInManagerMock = new Mock<SignInManager<IdentityUser>>(_userManagerMock.Object, contextAccessor.Object, userPrincipalFactory.Object, null, null, null, null);

            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);

            _configurationMock = new Mock<IConfiguration>();
            _deliverDriverServiceMock = new Mock<IDeliverDriverService>();
        }

        [Fact]
        public async Task Login_Should_Return_Ok()
        {
            var loginDto = new LoginDto { Username = "testuser", Password = "password" };
            _signInManagerMock.Setup(x => x.PasswordSignInAsync(loginDto.Username, loginDto.Password, false, false))
                              .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            _userManagerMock.Setup(x => x.FindByNameAsync(loginDto.Username))
                              .ReturnsAsync(new IdentityUser { UserName = loginDto.Username });
            _configurationMock.Setup(x => x["Jwt:Key"]).Returns("e83d2332-d140-45b6-b21c-d8f864a3327e");
            _configurationMock.Setup(x => x["Jwt:Issuer"]).Returns("YourIssuer");

            var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, _roleManagerMock.Object,
                _configurationMock.Object, _deliverDriverServiceMock.Object);

            var result = await controller.Login(loginDto);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Login_Should_Return_Unauthorized()
        {
            var loginDto = new LoginDto { Username = "testuser", Password = "password" };
            _signInManagerMock.Setup(x => x.PasswordSignInAsync(loginDto.Username, loginDto.Password, false, false))
                              .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);
            _userManagerMock.Setup(x => x.FindByNameAsync(loginDto.Username))
                              .ReturnsAsync(new IdentityUser { UserName = loginDto.Username });
            _configurationMock.Setup(x => x["Jwt:Key"]).Returns("e83d2332-d140-45b6-b21c-d8f864a3327e");
            _configurationMock.Setup(x => x["Jwt:Issuer"]).Returns("YourIssuer");

            var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, _roleManagerMock.Object,
                _configurationMock.Object, _deliverDriverServiceMock.Object);

            var result = await controller.Login(loginDto);

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task Register_Admin_Should_Return_Ok()
        {
            var registerModel = new RegisterDto { Username = "newuser", Password = "password", Email = "email@test.com" };
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), registerModel.Password))
                            .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), MotorRentalIdentityConstants.ADMIN_ROLE_NAME))
                .ReturnsAsync(IdentityResult.Success);
            _deliverDriverServiceMock.Setup(x => x.AddAsync(It.IsAny<DeliverDriver>()));

            var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object,
                _roleManagerMock.Object, _configurationMock.Object, _deliverDriverServiceMock.Object);

            var result = await controller.RegisterAdmin(registerModel);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Register_Admin_Should_Return_BadRequest()
        {
            var registerModel = new RegisterDto { Username = "newuser", Password = "password", Email = "email@test.com" };
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), registerModel.Password))
                            .ReturnsAsync(IdentityResult.Failed());
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), MotorRentalIdentityConstants.ADMIN_ROLE_NAME))
                .ReturnsAsync(IdentityResult.Success);
            _deliverDriverServiceMock.Setup(x => x.AddAsync(It.IsAny<DeliverDriver>()));

            var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object,
                _roleManagerMock.Object, _configurationMock.Object, _deliverDriverServiceMock.Object);

            var result = await controller.RegisterAdmin(registerModel);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_DeliverDriver_Should_Return_Ok()
        {
            var registerModel = new RegisterDeliverDriverDto { Username = "newuser", Password = "password", Email = "email@test.com" };
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), registerModel.Password))
                            .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), MotorRentalIdentityConstants.ADMIN_ROLE_NAME))
                .ReturnsAsync(IdentityResult.Success);

            var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object,
                _roleManagerMock.Object, _configurationMock.Object, _deliverDriverServiceMock.Object);

            var result = await controller.RegisterDeliverDriver(registerModel);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Register_DeliverDriver_Should_Return_BadRequest()
        {
            var registerModel = new RegisterDeliverDriverDto { Username = "newuser", Password = "password", Email = "email@test.com" };
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), registerModel.Password))
                            .ReturnsAsync(IdentityResult.Failed());
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), MotorRentalIdentityConstants.ADMIN_ROLE_NAME))
                .ReturnsAsync(IdentityResult.Success);

            var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object,
                _roleManagerMock.Object, _configurationMock.Object, _deliverDriverServiceMock.Object);

            var result = await controller.RegisterDeliverDriver(registerModel);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
