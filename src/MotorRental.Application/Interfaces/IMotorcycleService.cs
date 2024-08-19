using MotorRental.Application.Common;
using MotorRental.Domain.Dtos;
using MotorRental.Domain.Entities;

namespace MotorRental.Application.Interfaces
{
    public interface IMotorcycleService : IBaseService<Motorcycle>
    {
        Task<List<MotorcycleDto>> Get(GetMotorcyclesFilterDto getMotorcyclesFilterDto);
        Task<ApiResponse> AddMotorcycle(MotorcycleDto motorcycleDto);
        Task<ApiResponse> UpdateLicensePlate(UpdateLicensePlateDto updateLicensePlateRequest);
    }
}
