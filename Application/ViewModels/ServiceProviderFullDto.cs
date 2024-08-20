using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class ServiceProviderFullDto
    {


        public ServiceProviderDto? ServiceProviderDto { get; set; }
        public ServiceProviderAddressDto? ServiceProviderAddressDto { get; set; }
        public ServiceProviderGeoLocationDto? ServiceProviderGeoLocationDto { get; set; }


        //this is required if it is first time registration
        public string? Password { get; set; }
        public IFormFile? Image { get; set; }
    }
}