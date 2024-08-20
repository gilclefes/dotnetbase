using dotnetbase.Application.Database;
using dotnetbase.Application.Models;
using dotnetbase.Application.Startup;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace dotnetbase.Application.Services.Auth
{
    public class RolesService
    {
        private readonly DatabaseContext _db;
        private readonly UserManager<ApplicationUser> _userManager;


        public RolesService(DatabaseContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<List<Role>> FindUserRolesAsync(string userId)
        {
            var roles = await _db.Roles.Where(role => role.UserRoles.Any(x => x.UserId == userId)).ToListAsync();
            return roles;
        }

        public async Task<bool> IsUserInRole(string userId, string roleName)
        {
            var userRolesQuery = from role in _db.Roles
                                 where role.Name == roleName
                                 from user in role.UserRoles
                                 where user.UserId.Equals(userId.ToString(), StringComparison.CurrentCultureIgnoreCase)
                                 select role;
            var userRole = await userRolesQuery.FirstOrDefaultAsync();
            return userRole != null;
        }

        public async Task<List<ApplicationUser>> FindUsersInRoleAsync(string roleName)
        {

            return (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync(roleName);
        }
    }
}
