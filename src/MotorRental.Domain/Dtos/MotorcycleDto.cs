using System.ComponentModel.DataAnnotations;

namespace MotorRental.Domain.Dtos
{
    public class MotorcycleDto
    {
        public int Id { get; set; }
        [Required]
        public int Year { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Model { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string LicensePlate { get; set; }
    }
}
