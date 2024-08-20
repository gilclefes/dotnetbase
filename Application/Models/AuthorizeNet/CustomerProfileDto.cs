using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.Models.AuthorizeNet
{
    public class CustomerProfileDto
    {
        public int ClientId { get; set; }
        public required string Email { get; set; }

        public CardDto? CreditCard { get; set; }
        public BankAccountDto? BankAccount { get; set; }
        public CustomerAddressDto? BillTo { get; set; }

        public string? PayType { get; set; }

        // public bool isPreffered { get; set; }


    }
}