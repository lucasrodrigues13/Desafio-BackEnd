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

        public async Task AddMotorcycle(MotorcycleDto motorcycleDto)
        {
            var motorcycle = await _motorcyleRepository.AddAsync(new Motorcycle
            {
                LicensePlate = motorcycleDto.LicensePlate,
                Model = motorcycleDto.Model,
                Year = motorcycleDto.Year
            });

            _messagingService.SendMessage(RabbitMqConstants.MOTORCYCLE_NOTIFICATION_QUEUE_NAME, JsonConvert.SerializeObject(motorcycle));
        }

        public async Task UpdateLicensePlate(UpdateLicensePlateDto updateLicensePlateRequest)
        {
            var motorcycle = await GetByIdAsync(updateLicensePlateRequest.MotorcycleId);
            if (motorcycle != null)
            {
                motorcycle.LicensePlate = updateLicensePlateRequest.LicensePlate;
                await _motorcyleRepository.UpdateAsync(motorcycle);
            }
        }
    }
}
