using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class ClientActiveSubscriptionDetailDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? TermAndConditions { get; set; }

        public decimal SubscriptionPrice { get; set; }
        public string? UnitTypeName { get; set; }
        public string? PricePeriodName { get; set; }

        public DateTime ExpireDate { get; set; }

        public int SubscritpionPlanId { get; set; }

        public string Services { get; set; }
        public string Charges { get; set; }
        public string Detergents { get; set; }

        public string? ClientSubscriptionDetailStatus { get; set; }
    }
}