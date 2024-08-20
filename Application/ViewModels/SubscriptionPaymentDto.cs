using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class SubscriptionPaymentDto
    {
        public int clientId { get; set; }
        public int ClientSubscriptionDetailId { get; set; }
        public string? PlanName { get; set; }
        public decimal TotalAmount { get; set; }

    }
}