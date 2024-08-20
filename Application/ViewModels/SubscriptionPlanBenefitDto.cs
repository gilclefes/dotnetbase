using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class SubscriptionPlanBenefitDto
    {
        public int Id { get; set; }

        public int SubscriptionId { get; set; }
        public required string Benefit { get; set; }
        public int Rank { get; set; }
        public Boolean Status { get; set; }

        public string? SubscriptionPlanName { get; set; }

    }
}