using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spark.Library.Database;

namespace dotnetbase.Application.Models
{
    public class ContactUs : BaseModel
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public required string Message { get; set; }
        public bool Read { get; set; }

        public string? ReadBy { get; set; }
        public DateTime? ReadAt { get; set; }


    }
}