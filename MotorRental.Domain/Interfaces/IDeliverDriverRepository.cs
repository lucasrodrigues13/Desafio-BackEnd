using MotorRental.Domain.Entities;

namespace MotorRental.Domain.Interfaces
{
    public interface IDeliverDriverRepository : IBaseRepository<DeliverDriver>
    {
        bool VerifyExistsByColumn(Func<DeliverDriver, bool> filter);
    }
}
