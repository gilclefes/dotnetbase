using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spark.Library.Database;

namespace dotnetbase.Application.Models
{
    public class UserDeviceToken : BaseModel
    {
        public required string Email { get; set; }
        public required string DeviceToken { get; set; }
    }
}