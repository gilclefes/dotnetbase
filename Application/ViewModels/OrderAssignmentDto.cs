using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class OrderAssignmentDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public required string OrderAssignmentType { get; set; } // whether paid or subscription
        public required string AssingedUserName { get; set; }
        public required DateTime DateAssigned { get; set; }
        public string? Comments { get; set; }

        public string? AssignedStatus { get; set; } // whether accepted or rejected

        public DateTime? DateStatusChanged { get; set; }

        public string? OrderRefNumber { get; set; }
    }
}