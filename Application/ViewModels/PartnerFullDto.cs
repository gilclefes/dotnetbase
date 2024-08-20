using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class PartnerFullDto
    {
        public PartnerDto? partnerDto { get; set; }
        public PartnerAddressDto? partnerAddressDto { get; set; }
        public PartnerGeoLocationDto? partnerGeoLocationDto { get; set; }
        public IFormFile? Image { get; set; }
        //this is required if it is first time registration
        public string? Password { get; set; }
    }
}