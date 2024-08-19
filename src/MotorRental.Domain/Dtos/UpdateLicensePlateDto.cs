using System.ComponentModel.DataAnnotations;

namespace MotorRental.Domain.Dtos
{
    public class UpdateLicensePlateDto
    {
        [Required]
        public int MotorcycleId { get; set; }
        [Required]
        public string LicensePlate { get; set; }
    }
}
