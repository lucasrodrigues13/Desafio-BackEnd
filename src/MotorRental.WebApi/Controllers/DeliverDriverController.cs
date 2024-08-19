using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotorRental.Application.Interfaces;
using MotorRental.Domain.Constants;
using MotorRental.Domain.Dtos;
using System.Security.Claims;

namespace MotorRental.WebApi.Controllers
{
    [Authorize(Roles = $"{MotorRentalIdentityConstants.ADMIN_ROLE_NAME}, {MotorRentalIdentityConstants.DELIVER_DRIVER_ROLE_NAME}")]
    [Route("api/[controller]")]
    [ApiController]
    public class DeliverDriverController : ApplicationControllerBase
    {
        private readonly IDeliverDriverService _driverService;
        private readonly IRentalService _rentalService;
        public DeliverDriverController(IDeliverDriverService driverService, IRentalService rentalService)
        {
            _driverService = driverService;
            _rentalService = rentalService;
        }

        [HttpPost("UploadLicenseDriverPhoto")]
        public async Task<IActionResult> UploadLicenseDriverPhoto(UploadLicenseDriverPhotoDto uploadLicenseDriverPhotoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var apiResponse = await _driverService.UploadLicenseDriverPhotoAsync(uploadLicenseDriverPhotoDto);

            if (!apiResponse.Success)
                return BadRequest(apiResponse);

            return Ok(apiResponse);
        }

        [HttpPut("RentAMotorcycle")]
        public async Task<IActionResult> RentAMotorcycle(RentAMotorcycleDto rentAMotorcycleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var apiResponse = await _rentalService.RentAMotorcycle(rentAMotorcycleDto);
            if (!apiResponse.Success)
                return BadRequest(apiResponse);

            return Ok(apiResponse);
        }

        [HttpPut("InformEndDateRental")]
        public async Task<IActionResult> InformEndDateRental(InformEndDateRentalDto informEndDateRentalDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var apiResponse = await _rentalService.InformEndDateRental(informEndDateRentalDto);
            if (!apiResponse.Success)
                return BadRequest(apiResponse);

            return Ok(apiResponse);
        }
    }
}