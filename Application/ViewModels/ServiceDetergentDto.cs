using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class ServiceDetergentDto
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int DetergentId { get; set; }

        public decimal Price { get; set; }
        public string? ServiceName { get; set; }
        public string? DetergentName { get; set; }
    }
}