using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class ServiceDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public string? Code { get; set; }
        public Boolean? Status { get; set; }
        public string? Description { get; set; }
        public string? HowTo { get; set; }
        public int CategoryId { get; set; }
        public int TypeId { get; set; }
        public decimal MinOrderValue { get; set; }
        public string? ServiceCategoryName { get; set; }
        public string? ServiceTypeName { get; set; }

        public bool? Pending { get; set; } = false;
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
    }
}