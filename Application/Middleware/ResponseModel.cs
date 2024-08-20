using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.Middleware
{
    public class ResponseModel
    {
        public int responseCode { get; set; }
        public string? responseMessage { get; set; }
    }
}