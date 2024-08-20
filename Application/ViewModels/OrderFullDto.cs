using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class OrderFullDto
    {
        public required string ClientEmailAddress { get; set; }
        public required DateTime PickupDate { get; set; }
        public required List<OrderItemDto> OrderItems { get; set; }
        public List<OrderDetergentDto>? OrderDetergents { get; set; }
        public OrderLocationDto? OrderLocation { get; set; }
        public string? AssignedCleaner { get; set; }
        public string? PromoCode { get; set; }
    }
}