using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.Models.AuthorizeNet
{
    public class BankAccountDto
    {
        //Either checking, savings, or businessChecking

        //public required string AccountType { get; set; }
        public required string RoutingNumber { get; set; }
        public required string AccountNumber { get; set; }
        public required string NameOnAccount { get; set; }
        // public required string EcheckType { get; set; }
        // public required string BankName { get; set; }

        public int ClientId { get; set; }
        public bool IsDefault { get; set; }

    }
}