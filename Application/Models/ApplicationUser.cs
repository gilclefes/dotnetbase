using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace dotnetbase.Application.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            UserRoles = new HashSet<UserRole>();

        }
        public string? Password { get; set; }


        public string? RememberToken { get; set; }

        public DateTime? EmailVerifiedAt { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}