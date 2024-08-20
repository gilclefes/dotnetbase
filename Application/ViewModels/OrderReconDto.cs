using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class OrderReconDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        public required string PartnerUserName { get; set; }
        public required decimal TotalOrderAmount { get; set; }
        public required decimal YaboShare { get; set; }
        public required decimal PartnerShare { get; set; }
        public required DateTime OrdecCompletionDate { get; set; }

        public string? OrderRefNumber { get; set; }
    }
}