using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spark.Library.Database;

namespace dotnetbase.Application.Models
{
    public class UserVerificationCode : BaseModel
    {
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PhoneCode { get; set; }
        public DateTime PhoneCodeExpiry { get; set; }
    }
}