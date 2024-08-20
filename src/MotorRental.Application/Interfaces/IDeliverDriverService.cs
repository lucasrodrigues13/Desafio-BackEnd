using MotorRental.Application.Common;
using MotorRental.Domain.Dtos;
using MotorRental.Domain.Entities;

namespace MotorRental.Application.Interfaces
{
    public interface IDeliverDriverService : IBaseService<DeliverDriver>
    {
        Task<ApiResponse> RegisterAdmin(RegisterDto registerDeliverDriverDto);
        Task<ApiResponse> RegisterDeliverDriver(RegisterDeliverDriverDto registerDeliverDriverDto);
        Task<ApiResponse> UploadLicenseDriverPhotoAsync(UploadLicenseDriverPhotoDto uploadLicenseDriverPhotoDto);
    }
}
