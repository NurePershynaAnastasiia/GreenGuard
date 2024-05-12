﻿using System.ComponentModel.DataAnnotations;

namespace GreenGuard.Models.Worker
{
    public class WorkerRegister
    {
        [StringLength(100)]
        public required string WorkerName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        [StringLength(300)]
        public required string Email { get; set; }

        [MinLength(8)]
        [RegularExpression(@"^(?=.*\d).*$")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        public required bool IsAdmin { get; set; }
    }
}
