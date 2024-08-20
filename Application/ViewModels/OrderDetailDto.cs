using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnetbase.Application.Models;

namespace dotnetbase.Application.ViewModels
{
    public class OrderDetailDto
    {

        public required OrderDto OrderDto { get; set; }
        public required List<OrderItemDto> OrderItems { get; set; }
        public List<OrderDetergentDto>? OrderDetergents { get; set; }
        public OrderLocationDto? OrderLocation { get; set; }

        public OrderPaymentDto? OrderPayment { get; set; }
        public List<OrderMessageDto>? OrderMessages { get; set; }
        public List<OrderStatusUpdateDto>? OrderStatuses { get; set; }
        public List<OrderChargeDto>? OrderCharges { get; set; }
        public List<OrderRatingDto>? OrderRatings { get; set; }

        public List<OrderAssignmentDto>? orderAssignments { get; set; }
    }
}