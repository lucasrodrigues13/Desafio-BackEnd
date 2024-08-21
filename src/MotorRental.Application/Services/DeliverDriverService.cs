using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MotorRental.Application.Common;
using MotorRental.Application.Interfaces;
using MotorRental.Domain.Constants;
using MotorRental.Domain.Dtos;
using MotorRental.Domain.Entities;
using MotorRental.Domain.Interfaces;

namespace MotorRental.Application.Services
{
    public class DeliverDriverService : BaseService<DeliverDriver>, IDeliverDriverService
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<IdentityUser> _userManager;
        private readonly IAwsS3Service _awsS3Service;
        private readonly IDeliverDriverRepository _deliverDriverRepository;

        public DeliverDriverService(IAwsS3Service awsS3Service, IDeliverDriverRepository deliverDriverRepository, UserManager<IdentityUser> userManager) : base(deliverDriverRepository)
        {
            _awsS3Service = awsS3Service;
            _deliverDriverRepository = deliverDriverRepository;
            _userManager = userManager;
        }

        public async Task<ApiResponse> RegisterAdmin(RegisterDto registerDto)
        {
            var user = new IdentityUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email
            };

            var result = await RegisterUser(registerDto, user, MotorRentalIdentityConstants.ADMIN_ROLE_NAME);

            if (result == null || !result.Succeeded)
                return ApiResponse.BadRequest(result.Errors.Select(a => a.Description).ToList());

            return ApiResponse.Ok();
        }

        public async Task<ApiResponse> RegisterDeliverDriver(RegisterDeliverDriverDto registerDeliverDriverDto)
        {
            var errors = ValidDeliverDriverData(registerDeliverDriverDto);
            if (errors.Any())
                return ApiResponse.BadRequest(errors);

            var user = new IdentityUser
            {
                UserName = registerDeliverDriverDto.Username,
                Email = registerDeliverDriverDto.Email
            };
            var registerDto = new RegisterDto
            {
                Password = registerDeliverDriverDto.Password,
                Username = registerDeliverDriverDto.Username,
                Email = registerDeliverDriverDto.Email,
            };

            var result = await RegisterUser(registerDto, user, MotorRentalIdentityConstants.DELIVER_DRIVER_ROLE_NAME);

            if (!result.Succeeded)
                return ApiResponse.BadRequest(result.Errors.Select(a => a.Description).ToList());

            await _deliverDriverRepository.AddAsync(new DeliverDriver
            {
                BirthDate = registerDeliverDriverDto.BirthDate,
                Email = registerDeliverDriverDto.Email,
                Cnpj = registerDeliverDriverDto.Cnpj,
                FullName = registerDeliverDriverDto.FullName,
                LicenseDriverNumber = registerDeliverDriverDto.LicenseDriverNumber,
                LicenseDriverType = registerDeliverDriverDto.LicenseDriverType,
                IdentityUserId = user.Id
            });

            return ApiResponse.Ok();
        }
        public async Task<ApiResponse> UploadLicenseDriverPhotoAsync(UploadLicenseDriverPhotoDto uploadLicenseDriverPhotoDto)
        {
            var driver = await _deliverDriverRepository.GetByIdAsync(uploadLicenseDriverPhotoDto.DeliverDriverId);
            var errors = ValidImage(uploadLicenseDriverPhotoDto.LicenseDriverPhoto);
            if (driver == null)
                errors.Add(ErrorMessagesConstants.DELIVER_DRIVER_NOT_REGISTERED);

            if (errors.Count > 0)
                return new ApiResponse(false, ErrorMessagesConstants.BADREQUEST_DEFAULT, null, errors);

            var imagePath = $"licenses/{driver.Email}/{uploadLicenseDriverPhotoDto.DeliverDriverId}";

            using (var stream = uploadLicenseDriverPhotoDto.LicenseDriverPhoto.OpenReadStream())
                await _awsS3Service.UploadFileAsync(AwsConstants.S3_BUCKET_NAME, imagePath, stream);

            driver.LicenseDriverImagePath = imagePath;
            await _deliverDriverRepository.UpdateAsync(driver);

            return ApiResponse.Ok();
        }
        private List<string> ValidImage(IFormFile licenseDriverPhoto)
        {
            var errors = new List<string>();

            if (licenseDriverPhoto == null || licenseDriverPhoto.Length == 0)
            {
                errors.Add(ErrorMessagesConstants.PHOTO_EMPTY);
                return errors;
            }

            var fileExtension = Path.GetExtension(licenseDriverPhoto.FileName).ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(licenseDriverPhoto.FileName) || (fileExtension != ".png" && fileExtension != ".bmp"))
                errors.Add(ErrorMessagesConstants.PHOTO_EXTENSION_INVALID);

            return errors;
        }
        private List<string> ValidDeliverDriverData(RegisterDeliverDriverDto registerDeliverDriverDto)
        {
            var errors = new List<string>();
            if (_deliverDriverRepository.VerifyExistsByColumn(a => a.Cnpj.Equals(registerDeliverDriverDto.Cnpj)))
                errors.Add(ErrorMessagesConstants.DELIVER_DRIVER_CNPJ_EXISTS);

            if (_deliverDriverRepository.VerifyExistsByColumn(a => a.LicenseDriverNumber.Equals(registerDeliverDriverDto.LicenseDriverNumber)))
                errors.Add(ErrorMessagesConstants.DELIVER_DRIVER_LICENSE_NUMBER);

            DateTime today = DateTime.Today;
            int age = today.Year - registerDeliverDriverDto.BirthDate.Year;
            if (registerDeliverDriverDto.BirthDate.Date > today.AddYears(-age))
                age--;

            if (age < 18)
                errors.Add(ErrorMessagesConstants.DELIVER_DRIVER_ILEGAL_AGE);

            return errors;
        }
        private async Task<Microsoft.AspNetCore.Identity.IdentityResult?> RegisterUser(RegisterDto registerDto, IdentityUser user, string role)
        {
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
                await _userManager.AddToRoleAsync(user, role);

            return result;
        }
    }
}