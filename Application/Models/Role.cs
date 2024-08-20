using Microsoft.AspNetCore.Identity;
using Spark.Library.Database;

namespace dotnetbase.Application.Models
{
    public class Role : IdentityRole
    {
        public Role()
        {
            UserRoles = new HashSet<UserRole>();
        }

        //  public string Name { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
