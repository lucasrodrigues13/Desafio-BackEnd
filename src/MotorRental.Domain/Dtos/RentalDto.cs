namespace MotorRental.Domain.Dtos
{
    public class RentalDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? ExpectedEndDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Price { get; set; }
        public decimal? ExpectedPrice { get; set; }

        public int DeliverDriverId { get; set; }
        public int PlanId { get; set; }
        public int MotorcycleId { get; set; }
    }
}
