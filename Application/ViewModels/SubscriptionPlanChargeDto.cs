using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class SubscriptionPlanChargeDto
    {
        public int Id { get; set; }
        public int ChargeId { get; set; }
        public int SubscriptionId { get; set; }
        public required decimal Amount { get; set; }
        public string? Description { get; set; }

        public Boolean Status { get; set; }

        public string? ChargeName { get; set; }

        public string? ChargeAmountType { get; set; }

        public string? SubscriptionPlanName { get; set; }
    }
}