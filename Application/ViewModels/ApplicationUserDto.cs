using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class ApplicationUserDto
    {

        public required string Id { get; set; }
        public required string UserName { get; set; }
        public string? NormalizedUserName { get; set; }
        public required string Email { get; set; }

        public string? NormalizedEmail { get; set; }
        public required bool EmailConfirmed { get; set; }

        public IList<string> Roles { get; set; }
    }
}