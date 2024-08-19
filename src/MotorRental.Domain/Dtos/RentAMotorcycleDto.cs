using System.ComponentModel.DataAnnotations;

namespace MotorRental.Domain.Dtos
{
    public class RentAMotorcycleDto
    {
        [Required]
        public int PlanId { get; set; }
        [Required]
        public int MotorcycleId { get; set; }
        [Required]
        public int DeliverDriverId { get; set; }
        public DateTime StartDate { get; } = DateTime.Now.AddDays(1);
    }
}
