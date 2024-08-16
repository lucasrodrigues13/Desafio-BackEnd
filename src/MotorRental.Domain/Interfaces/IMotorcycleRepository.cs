using MotorRental.Domain.Dtos;
using MotorRental.Domain.Entities;

namespace MotorRental.Domain.Interfaces
{
    public interface IMotorcycleRepository : IBaseRepository<Motorcycle>
    {
        Task<List<MotorcycleDto>> GetByLicensePlate(string licensePlate);
    }
}
