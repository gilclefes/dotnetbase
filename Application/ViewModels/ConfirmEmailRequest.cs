using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class ConfirmEmailRequest
    {
        public required string UserId { get; set; }
        public required string Token { get; set; }

    }
}