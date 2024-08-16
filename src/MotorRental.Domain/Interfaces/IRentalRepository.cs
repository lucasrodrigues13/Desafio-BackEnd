using MotorRental.Domain.Dtos;
using MotorRental.Domain.Entities;

namespace MotorRental.Domain.Interfaces
{
    public interface IRentalRepository : IBaseRepository<Rental>
    {
        Task<List<RentalDto>> Get();
    }
}
