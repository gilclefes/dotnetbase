using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class ChargeDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public required string Code { get; set; }
        public Boolean? Status { get; set; }
        public string? Description { get; set; }

        public Boolean IsYaboCharge { get; set; } = false;

        public required string AmountType { get; set; }//percentage or fixed

        public required int CategoryId { get; set; }

        public string? ChargeCategoryName
        {
            get; set;
        }
    }
}