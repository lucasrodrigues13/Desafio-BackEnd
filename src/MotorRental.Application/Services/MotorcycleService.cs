using MotorRental.Application.Common;
using MotorRental.Application.Interfaces;
using MotorRental.Domain.Constants;
using MotorRental.Domain.Dtos;
using MotorRental.Domain.Entities;
using MotorRental.Domain.Interfaces;
using Newtonsoft.Json;

namespace MotorRental.Application.Services
{
    public class MotorcycleService : BaseService<Motorcycle>, IMotorcycleService
    {
        private readonly IMotorcycleRepository _motorcyleRepository;
        private readonly IMessagingService _messagingService;
        public MotorcycleService(IMotorcycleRepository motorcyleRepository, IMessagingService messagingService) : base(motorcyleRepository)
        {
            _motorcyleRepository = motorcyleRepository;
            _messagingService = messagingService;
        }

        public async Task<List<MotorcycleDto>> Get(GetMotorcyclesFilterDto getMotorcyclesFilterDto)
        {
            return await _motorcyleRepository.GetByLicensePlate(getMotorcyclesFilterDto.LicensePlate);
        }

        public async Task<ApiResponse> AddMotorcycle(MotorcycleDto motorcycleDto)
        {
            var errors = await ValidLicensePlate(motorcycleDto.LicensePlate);
            if (errors.Any())
                return ApiResponse.BadRequest(errors);

            var motorcycle = await _motorcyleRepository.AddAsync(new Motorcycle
            {
                LicensePlate = motorcycleDto.LicensePlate,
                Model = motorcycleDto.Model,
                Year = motorcycleDto.Year.Value
            });

            _messagingService.SendMessage(RabbitMqConstants.MOTORCYCLE_NOTIFICATION_QUEUE_NAME, JsonConvert.SerializeObject(motorcycle));

            return ApiResponse.Ok(motorcycle);
        }

        public async Task<ApiResponse> UpdateLicensePlate(UpdateLicensePlateDto updateLicensePlateRequest)
        {
            var errors = await ValidLicensePlate(updateLicensePlateRequest.LicensePlate);
            if (errors.Any())
                return ApiResponse.BadRequest(errors);

            var motorcycle = await GetByIdAsync(updateLicensePlateRequest.MotorcycleId);
            if (motorcycle != null)
            {
                motorcycle.LicensePlate = updateLicensePlateRequest.LicensePlate;
                await _motorcyleRepository.UpdateAsync(motorcycle);
            }

            return ApiResponse.Ok(motorcycle);
        }

        private async Task<List<string>> ValidLicensePlate(string licensePlate)
        {
            var errors = new List<string>();
            var motorcycleDb = await _motorcyleRepository.GetByLicensePlate(licensePlate);

            if (motorcycleDb != null)
                errors.Add(ErrorMessagesConstants.MOTORCYCLE_LICENSE_PLATE_EXISTS);

            return errors;
        }
    }
}
