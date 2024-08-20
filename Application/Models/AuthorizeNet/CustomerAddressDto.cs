using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.Models.AuthorizeNet
{
    public class CustomerAddressDto
    {
        public required string Address { get; set; }
        public required string City { get; set; }
        public required string Zip { get; set; }

        public int ClientId { get; set; }

        public string? State { get; set; }
        public string? Country { get; set; }
        public required string PhoneNumber { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
    }
}