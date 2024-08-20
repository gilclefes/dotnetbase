using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class ServiceChargeDto
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int ChargeId { get; set; }

        public decimal Price { get; set; }

        public string? ServiceName { get; set; }
        public string? ChargeName { get; set; }
        public string? ChargeAmountType { get; set; }

    }
}