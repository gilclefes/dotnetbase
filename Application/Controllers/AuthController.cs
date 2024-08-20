using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using dotnetbase.Application.Services.Auth;
using dotnetbase.Application.Events;
using Coravel.Events.Interfaces;
using dotnetbase.Application.Models;
using dotnetbase.Application.ViewModels;
using Microsoft.AspNetCore.Identity;
using dotnetbase.Application.Services;
using System.IdentityModel.Tokens.Jwt;
using dotnetbase.Application.Database;
using Microsoft.EntityFrameworkCore;
using dotnetbase.Application.Filter;
using dotnetbase.Application.Helpers;

namespace dotnetbase.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly AuthService _authService;
        private readonly IDispatcher _dispatcher;

        private readonly UsersService _userService;

        private readonly IUriService uriService;
        private readonly RoleManager<Role> _roleManager;

        private readonly DatabaseContext _context;



        private readonly UserManager<ApplicationUser> _userManager;




        public AuthController(UsersService usersService,
            IDispatcher dispatcher, UserManager<ApplicationUser> userManager,
            AuthService authService, RoleManager<Role> roleManager, DatabaseContext context, IUriService uriService)
        {

            _dispatcher = dispatcher;
            _authService = authService;
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = usersService;
            _context = context;
            this.uriService = uriService;


        }

        //[Authorize]
        [HttpGet("GetAllRoles")]
        [Authorize]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            return Ok(roles);
        }

        //return all users and pagination
        [Authorize]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            //select a list of users using the _usermanage with each use having its list of roles



            var pagedData = await _userManager.Users
               .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).AsNoTracking()
               .ToListAsync();
            var totalRecords = await _context.Clients.CountAsync();

            var usersWithRoles = new List<ApplicationUserDto>();

            foreach (var user in pagedData)
            {
                var roles = await _userManager.GetRolesAsync(user);
                usersWithRoles.Add(new ApplicationUserDto
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    EmailConfirmed = user.EmailConfirmed,
                    UserName = user.UserName ?? "",
                    NormalizedEmail = user.NormalizedEmail,
                    NormalizedUserName = user.NormalizedUserName,
                    Roles = roles
                });
            }


            var pagedReponse = PaginationHelper.CreatePagedReponse<ApplicationUserDto>(usersWithRoles, validFilter, totalRecords, uriService, route);
            return Ok(pagedReponse);
        }


        [HttpPost, AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login(Login request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request == null)
            {
                return BadRequest("user is not set.");
            }




            var user = await _userManager.FindByNameAsync(request.Email);
            if (user == null
                || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                ModelState.AddModelError("FailedLogin", "Login Failed: Your email or password was incorrect");
                return BadRequest(ModelState);
            }

            //check if user email is  verified
            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError("FailedLogin", "Login Failed: Your email is not yet verified");
                return BadRequest(ModelState);
            }

            var token = await _authService.CreateJwtToken(user);

            //get roles for user

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                username = user.UserName,
                email = user.Email,
                token = token,
                Success = true,
                Message = "Login successful",
                Roles = roles.ToArray(),

            });


        }

        [HttpPost, AllowAnonymous]
        [Route("register")]
        public async Task<IActionResult> Register(Register request)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request == null)
            {
                return BadRequest("user is not set.");
            }



            var existingUser = await _userManager.FindByNameAsync(request.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError("EmailExists", "Email already in use by another account.");
                return BadRequest(ModelState);
            }

            var user = _userService.CreateUserAsync(request);

            return Ok(new
            {
                Success = true,
                Message = "User registered!",
                Data = user,
            });

        }



        [HttpPost]
        [AllowAnonymous]
        [Route("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest request)
        {
            if (request.UserId == null || request.Token == null)
            {
                return BadRequest("Missing user ID or token.");
            }

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "User not found.",
                });

            }

            var result = await _userManager.ConfirmEmailAsync(user, request.Token);
            if (result.Succeeded)
            {
                return Ok(new
                {
                    Success = true,
                    Message = "Email confirmed successfully!",
                });
            }

            string errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new
            {
                Success = false,
                Message = $"Email not confirmed. Errors: {errors}",
            });
        }



        [HttpPost, AllowAnonymous]
        [Route("resendconfirmationemail")]
        public async Task<IActionResult> ResendConfirmationEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "User not found.",
                });

            }

            if (user.EmailConfirmed)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Email Already Confirmed"
                });
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            //var phoneToken = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);

            var emailVerfication = new UserEmailVerification(user, token);
            await _dispatcher.Broadcast(emailVerfication);

            return Ok(new
            {
                Success = true,
                Message = "Email confirmation sent!",
            });
        }

        [HttpPost, AllowAnonymous]
        [Route("forgotpassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                // User not found. You might want to handle this case differently in a real application.
                return BadRequest("User not found");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // broadcast user created event
            var userForgottenPassword = new UserForgottenPassword(user, token);
            await _dispatcher.Broadcast(userForgottenPassword);

            return Ok(new
            {
                Success = true,
                Message = "Token send to user email",
                Token = token,
            });

        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                // User not found. You might want to handle this case differently in a real application.
                return NotFound();
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (!result.Succeeded)
            {
                // Reset password failed. You might want to handle this case differently in a real application.
                return BadRequest(result.Errors);
            }

            return Ok(new
            {
                Success = true,
                Message = "Password reset successfully!",
            });
        }


        [HttpPost("UpdateRole")]
        [Authorize]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleDto model)
        {


            var existingUser = await _userManager.FindByNameAsync(model.Email);

            if (existingUser == null)
            {
                ModelState.AddModelError("User Does not Exist", "Email does not exist for any user");
                return BadRequest(ModelState);
            }

            //get roles for user that are not in model.roles

            var userRoles = await _userManager.GetRolesAsync(existingUser);

            if (model.Roles != null && model.Roles.Any())
            {

                var rolesToRemove = userRoles.Where(x => !model.Roles.Contains(x)).Select(x => x);

                var rolesToAdd = model.Roles.Where(x => !userRoles.Contains(x)).Select(x => x);

                if (rolesToAdd.Any())
                {
                    foreach (var role in rolesToAdd)
                    {
                        await _userManager.AddToRoleAsync(existingUser, role);
                    }
                }

                if (rolesToRemove.Any())
                {
                    foreach (var role in rolesToRemove)
                    {
                        await _userManager.RemoveFromRoleAsync(existingUser, role);
                    }
                }

                await _context.SaveChangesAsync();

            }











            return Ok(new
            {
                Success = true,
                Message = "User Roles Updated",
            });

        }

    }


}
