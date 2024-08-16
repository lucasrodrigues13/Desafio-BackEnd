namespace MotorRental.Domain.Dtos
{
    public class ErrorDto
    {
        public string Message { get; set; }
        public string Detail { get; set; }
        public int? Status { get; set; }
    }
}
