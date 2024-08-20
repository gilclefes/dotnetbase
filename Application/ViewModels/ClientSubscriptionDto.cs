using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class ClientSubscriptionDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int SubscriptionId { get; set; }


        public Boolean Status { get; set; }
        public DateTime FirstSubscriptionDate { get; set; }
        public DateTime LastRenewedDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        public string? ClientName { get; set; }
        public string? SubscriptionPlanName { get; set; }
        public int? SubscriptionPlanDetailId { get; set; }
        public int? SubscriptionPlanPriceId { get; set; }
    }
}