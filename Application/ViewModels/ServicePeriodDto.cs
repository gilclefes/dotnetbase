using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class ServicePeriodDto
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int PeriodId { get; set; }

        public string? ServiceName { get; set; }
        public string? PeriodName { get; set; }
    }
}