using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class SubscriptionFullDto
    {
        public SubscriptionPlanDto? subscriptionPlanDto { get; set; }
        public List<SubscriptionPlanChargeDto>? subscriptionPlanChargeDto { get; set; }
        public List<SubscriptionPlanChargeExemptionDto>? subscriptionPlanChargeExemptionDto { get; set; }
        public List<SubscriptionPlanServiceDto>? subscriptionPlanServiceDto { get; set; }
        public int Id { get; set; }
        public List<SubscriptionPlanPriceDto>? subscriptionPlanPriceDto { get; set; }
        public List<SubscriptionPlanBenefitDto>? subscriptionPlanBenefitDto { get; set; }
    }
}