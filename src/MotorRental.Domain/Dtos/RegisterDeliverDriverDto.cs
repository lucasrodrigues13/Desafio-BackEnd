using MotorRental.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MotorRental.Domain.Dtos
{
    public class RegisterDeliverDriverDto
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Cnpj { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]
        public long LicenseDriverNumber { get; set; }
        [Required]
        public LicenseDriverTypeEnum LicenseDriverType { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
