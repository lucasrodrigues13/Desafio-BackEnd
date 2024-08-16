using Microsoft.AspNetCore.Mvc;
using MotorRental.Application.Interfaces;

namespace MotorRental.WebApi.Controllers
{
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
