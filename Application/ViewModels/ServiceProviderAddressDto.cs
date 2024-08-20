using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class ServiceProviderAddressDto
    {
        public int Id { get; set; }
        public int ServiceProviderId { get; set; }
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? StateProvince { get; set; }
        public string? CountryRegion { get; set; }
        public string? PostalCode { get; set; }

        public Boolean Status { get; set; }

        public string? ServiceProviderName { get; set; }
    }
}