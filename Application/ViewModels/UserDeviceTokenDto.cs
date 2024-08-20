using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class UserDeviceTokenDto
    {
        public required string Email { get; set; }
        public required string DeviceToken { get; set; }
    }
}