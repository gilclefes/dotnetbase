using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class PriceDto
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int ItemId { get; set; }
        public int PeriodId { get; set; }
        public int UnitTypeId { get; set; }

        public decimal Amount { get; set; }

        public Boolean? Status { get; set; }

        public string? ServiceName { get; set; }
        public string? ItemName { get; set; }
        public string? PeriodName { get; set; }
        public string? UnitTypeName { get; set; }
        public string? ItemTypeName { get; set; }
    }
}