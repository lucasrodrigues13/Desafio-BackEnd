using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotorRental.Application.Interfaces;
using MotorRental.Domain.Constants;

namespace MotorRental.WebApi.Controllers
{
    [Authorize(Roles = $"{MotorRentalIdentityConstants.ADMIN_ROLE_NAME}, {MotorRentalIdentityConstants.DELIVER_DRIVER_ROLE_NAME}")]
    [Route("api/[controller]")]
    [ApiController]
    public class RentalController : ApplicationControllerBase
    {
        private readonly IRentalService _rentalService;
        public RentalController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var rentals = await _rentalService.Get();

            if (!rentals.Any())
                return NoContent();

            return Ok(rentals);
        }
    }
}
