using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotorRental.Application.Interfaces;
using MotorRental.Domain.Constants;
using MotorRental.Domain.Dtos;

namespace MotorRental.WebApi.Controllers
{
    [Authorize(Roles = MotorRentalIdentityConstants.ADMIN_ROLE_NAME)]
    [Route("api/[controller]")]
    [ApiController]
    public class MotorcycleController : ApplicationControllerBase
    {
        private readonly IMotorcycleService _motorcycleService;
        public MotorcycleController(IMotorcycleService service, ILogger<MotorcycleController> logger)
        {
            _motorcycleService = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetMotorcyclesFilterDto getMotorcyclesFilterDto)
        {
            var motorcycles = await _motorcycleService.Get(getMotorcyclesFilterDto);

            if (!motorcycles.Any())
                return NoContent();

            return Ok(motorcycles);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MotorcycleDto motorcycleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var apiResponse = await _motorcycleService.AddMotorcycle(motorcycleDto);

            if (!apiResponse.Success)
                return BadRequest(apiResponse);

            return Ok(apiResponse);
        }

        [HttpPatch("UpdateLicensePlate")]
        public async Task<IActionResult> UpdateLicensePlate(UpdateLicensePlateDto updateLicensePlateRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var apiResponse = await _motorcycleService.UpdateLicensePlate(updateLicensePlateRequest);

            if (!apiResponse.Success)
                return BadRequest(apiResponse);

            return Ok(apiResponse);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            await _motorcycleService.DeleteByIdAsync(id);

            return Ok();
        }
    }
}
