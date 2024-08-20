using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ServiceId { get; set; }
        public int ItemId { get; set; }

        public int PeriodId { get; set; }

        public int UnitTypeId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal NetPrice { get; set; }

        public string? Comments { get; set; }
        public string? OrderName { get; set; }
        public string? ServiceName { get; set; }
        public string? LaundryItemName { get; set; }
        public string? UnitTypeName { get; set; }
    }
}