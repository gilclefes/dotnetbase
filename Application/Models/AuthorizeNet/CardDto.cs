using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.Models.AuthorizeNet
{
    public class CardDto
    {
        public required string CardNumber { get; set; }
        public required string ExpirationDate { get; set; }
        public required string IssuerNumber { get; set; }



        public int ClientId { get; set; }

        public bool IsDefault { get; set; }
    }
}