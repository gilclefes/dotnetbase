using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class SubscriptionPlanPriceDto
    {
        public int Id { get; set; }
        public int SubscriptionId { get; set; }
        public int PeriodId { get; set; }

        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }

        public string? Description { get; set; }

        public Boolean Status { get; set; }
        public Boolean? IsFavorite { get; set; } = false;

        public string? SubscriptionPlanName { get; set; }
        public string? PeriodName { get; set; }
        public string? CurrencyName { get; set; }
    }
}