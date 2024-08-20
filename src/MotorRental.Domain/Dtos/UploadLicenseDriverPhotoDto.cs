using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MotorRental.Domain.Dtos
{
    public class UploadLicenseDriverPhotoDto
    {
        [Required]
        public int DeliverDriverId { get; set; }
        [Required]
        public IFormFile LicenseDriverPhoto { get; set; }
    }
}
