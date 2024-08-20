using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class OrderRatingDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public required string RaterEmail { get; set; }
        public required string RatedEmail { get; set; }
        public int Rating { get; set; }
        public string? Message { get; set; }

        public string? OrderRefNumber { get; set; }
    }
}