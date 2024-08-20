using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class OrderStatsDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSales { get; set; }
        public int TotalCancelledOrders { get; set; }
        public int TotalCompletedOrders { get; set; }
        public int TotalClients { get; set; }
        public int? TotalSubscriptions { get; set; }


    }
}