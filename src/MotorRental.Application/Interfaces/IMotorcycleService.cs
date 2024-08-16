using MotorRental.Domain.Dtos;
using MotorRental.Domain.Entities;

namespace MotorRental.Application.Interfaces
{
    public interface IMotorcycleService : IBaseService<Motorcycle>
    {
        Task<List<MotorcycleDto>> Get(GetMotorcyclesFilterDto getMotorcyclesFilterDto);
        Task UpdateLicensePlate(UpdateLicensePlateDto updateLicensePlateRequest);
        Task AddMotorcycle(MotorcycleDto motorcycleDto);
    }
}
