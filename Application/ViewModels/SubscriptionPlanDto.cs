using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class SubscriptionPlanDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? TermAndConditions { get; set; }
        public Boolean Status { get; set; }

        public int OrderPeriodId { get; set; }
        public int OrderFrequency { get; set; }

        public int MinOrder { get; set; }

        public decimal? MinOrderPenalty { get; set; }

        public string? PeriodName { get; set; }
    }
}