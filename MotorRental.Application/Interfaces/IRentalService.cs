using MotorRental.Application.Common;
using MotorRental.Domain.Dtos;
using MotorRental.Domain.Entities;

namespace MotorRental.Application.Interfaces
{
    public interface IRentalService : IBaseService<Rental>
    {
        Task<List<RentalDto>> Get();
        Task<ApiResponse> RentAMotorcycle(RentAMotorcycleDto rentAMotorcycleDto);
        Task<ApiResponse> InformEndDateRental(InformEndDateRentalDto informEndDateRentalDto);
    }
}
