using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class ServiceProviderDto
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public int IdTypeId { get; set; }
        public required string IdNumber { get; set; }

        public string? Logo { get; set; }
        public string? IdTypeName { get; set; }

        public int RegStatusId { get; set; }

        public decimal? Rating { get; set; }

        public string? RegStatusName { get; set; }

        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? StateProvince { get; set; }
        public string? CountryRegion { get; set; }
        public string? PostalCode { get; set; }

        public string? Code { get; set; }
    }
}