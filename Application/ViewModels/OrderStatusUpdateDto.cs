using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class OrderStatusUpdateDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        public int OrderStatusId { get; set; }
        public string? Status { get; set; }
        public DateTime StatusDate { get; set; }
        public string? UpdatedBy { get; set; }
        public string? Description { get; set; }
    }
}