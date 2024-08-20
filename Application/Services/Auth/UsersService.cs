using dotnetbase.Application.Database;
using dotnetbase.Application.Models;
using dotnetbase.Application.Startup;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Spark;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using dotnetbase.Application.ViewModels;
using Coravel.Events.Interfaces;
using dotnetbase.Application.Events;
using dotnetbase.Application.Wrappers;
using dotnetbase.Application.Mail;
using Spark.Library.Mail;
using System.Web;


namespace dotnetbase.Application.Services.Auth
{
    public class UsersService
    {
        private readonly DatabaseContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IDispatcher _dispatcher;

        private readonly string _userRoleId;

        private readonly IMailer _mailer;

        private readonly IConfiguration _config;
        public UsersService(IMailer mailer, IConfiguration config, IDispatcher dispatcher, DatabaseContext db, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _db = db;
            _userManager = userManager;
            _userRoleId = configuration.GetValue<string>("USER_ROLE_ID") ?? "";
            _dispatcher = dispatcher;
            _config = config;
            this._mailer = mailer;
        }

        public async Task<ApplicationUser?> FindUserAsync(int userId)
        {
            // Declare _config as a local variable
            return await _db.ApplicationUsers.FindAsync(userId);
        }

        public async Task<ApplicationUser?> FindUserAsync(string username, string password)
        {

            var user = await _userManager.FindByNameAsync(username);
            if (user == null
                || !await _userManager.CheckPasswordAsync(user, password))
            {
                return null;
            }

            return user;
        }

        public async Task<ApplicationUser?> FindUserByEmailAsync(string email)
        {
            return await _db.ApplicationUsers.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<ApplicationUser> CreateUserAsync(Register request)
        {

            var userForm = new ApplicationUser()
            {
                UserName = request.Email,
                Email = request.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = true,
                LockoutEnabled = false,
            };

            await _userManager.CreateAsync(userForm, request.Password);

            await _userManager.AddToRoleAsync(userForm, "User");

            await _db.SaveChangesAsync();

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(userForm);
            var userCreated = new UserCreated(userForm);
            var emailVerfication = new UserEmailVerification(userForm, token);
            await _dispatcher.Broadcast(emailVerfication);
            await _dispatcher.Broadcast(userCreated);
            return userForm;
        }




        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            return await _db.ApplicationUsers.AsNoTracking()
                    .ToListAsync();
        }

        public string GetSha256Hash(string input)
        {
            using (var hashAlgorithm = SHA256.Create())
            {
                var byteValue = Encoding.UTF8.GetBytes(input);
                var byteHash = hashAlgorithm.ComputeHash(byteValue);
                return Convert.ToBase64String(byteHash);
            }
        }
    }
}
