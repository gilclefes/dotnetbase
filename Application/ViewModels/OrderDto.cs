using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class OrderDto
    {
        public int Id { get; set; }
        public required string RefNumber { get; set; }
        public required string ClientUserName { get; set; }

        public required DateTime OrderDate { get; set; }
        public required DateTime DeliveryDate { get; set; }

        public required DateTime PickupDate { get; set; }

        public required decimal GrossAmount { get; set; }
        public required decimal NetAmount { get; set; }
        public decimal TaxAmount { get; set; }

        public required decimal Discount { get; set; }

        public int OrderStatusId { get; set; }

        public string? OrderStatusName { get; set; }
    }
}