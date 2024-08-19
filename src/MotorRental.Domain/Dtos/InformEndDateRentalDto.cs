using System.ComponentModel.DataAnnotations;

namespace MotorRental.Domain.Dtos
{
    public class InformEndDateRentalDto
    {
        [Required]
        public int RentalId { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
    }
}
