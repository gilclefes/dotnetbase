using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.ViewModels
{
    public class UpdateRoleDto
    {
        public required string Email { get; set; }

        public required List<string> Roles { get; set; }
    }
}