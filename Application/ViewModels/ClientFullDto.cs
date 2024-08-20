using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class ClientFullDto
    {
        public ClientDto? clientDto { get; set; }
        public ClientAddressDto? clientAddressDto { get; set; }
        public ClientGeoLocationDto? clientGeoLocationDto { get; set; }
        public IFormFile? image { get; set; }
    }
}