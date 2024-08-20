using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public required string RefNumber { get; set; }
        public required string PaymentMethod { get; set; }
        public string? ExTransactionId { get; set; }
        public required DateTime PaymentStart { get; set; }
        public DateTime PaymentEnd { get; set; }
        public required decimal PaymentAmount { get; set; }

        public int CurrencyId { get; set; }

        public required string PaymentStatus { get; set; }

        public string? CurrencyName { get; set; }
        public string? CurrencyCode { get; set; }

        public required string PaymentType { get; set; }
        public string? Name { get; set; }

        public string? Descripion { get; set; }
    }
}