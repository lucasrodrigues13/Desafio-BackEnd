﻿using System.ComponentModel.DataAnnotations;

namespace MotorRental.Domain.Dtos
{
    public class LoginDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
