using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class OrderPaymentDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        public int PaymentId { get; set; }

        public string? OrderRefNumber { get; set; }

        public string? PaymentRefNumber { get; set; }


        public string? PaymentMethod { get; set; }
        public string? ExTransactionId { get; set; }

        public decimal? AmountPaid { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}