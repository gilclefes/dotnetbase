using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class OrderChargeDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ChargeId { get; set; }
        public DateTime DateAdded { get; set; }
        public string? ChargeDescription { get; set; }
        public required decimal Amount { get; set; }


        public string? OrderRefNumber { get; set; }
        public string? ChargeName { get; set; }
    }
}