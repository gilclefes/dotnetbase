using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.Models.AuthorizeNet
{
    public class AuthorizeNetCustomerProfile
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        public required int ClientId { get; set; }

        public string? CardPaymentProfileId { get; set; }
        public string? BankAccountPaymentProfileId { get; set; }
        public string? ShippingAddressId { get; set; }
        public string? CustomerProfileId { get; set; }

        public string? DefaultPaymentProfileId { get; set; }
    }
}