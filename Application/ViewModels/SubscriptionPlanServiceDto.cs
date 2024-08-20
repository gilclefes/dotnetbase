using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class SubscriptionPlanServiceDto
    {
        public int Id { get; set; }
        public int SubscriptionId { get; set; }
        public int ServiceId { get; set; }

        public string? Description { get; set; }

        public Boolean Status { get; set; }

        public string? SubscriptionPlanName { get; set; }
        public string? ServiceName { get; set; }
    }
}