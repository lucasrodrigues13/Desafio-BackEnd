using MotorRental.Domain.Entities;

namespace MotorRental.Domain.Interfaces
{
    public interface IMotorcycleRepository : IBaseRepository<Motorcycle>
    {
        Motorcycle? GetNextAvailableMotorcycle();
    }
}
