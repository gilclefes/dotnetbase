using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class OrderPromoCodeDto
    {
        public int OrderId { get; set; }

        public int PromoId { get; set; }



        public required Decimal TotalAmount { get; set; }

        public string? Comments { get; set; }

        public string? OrderRefNumber { get; set; }
        public string? PromoCodeName { get; set; }
    }
}