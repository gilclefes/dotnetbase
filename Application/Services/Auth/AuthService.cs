using Microsoft.IdentityModel.Tokens;
using dotnetbase.Application.Models;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace dotnetbase.Application.Services.Auth
{
	public class AuthService
	{
		private readonly UsersService _usersService;
		private readonly RolesService _rolesService;
		private readonly IConfiguration _configuration;
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly UserManager<ApplicationUser> _userManager;
		private HttpContext _context;

		public HttpContext Context
		{
			get
			{
				var context = _context ?? _contextAccessor?.HttpContext;
				if (context == null)
				{
					throw new InvalidOperationException("HttpContext must not be null.");
				}
				return context;
			}
			set
			{
				_context = value;
			}
		}

		public AuthService(RolesService rolesService, IConfiguration config, IHttpContextAccessor contextAccessor, UsersService usersService, UserManager<ApplicationUser> userManager)
		{
			_rolesService = rolesService;
			_configuration = config;
			_contextAccessor = contextAccessor;
			_usersService = usersService;
			_userManager = userManager;
		}

		public async Task<ApplicationUser?> GetAuthenticatedUser(ClaimsPrincipal User)
		{
			if (User.Identity != null && User.Identity.IsAuthenticated)
			{
				var userId = this.GetAuthenticatedUserId(User);
				if (userId != null)
				{
					var id = userId ?? default(int);
					return await _usersService.FindUserAsync(id);
				}
				return null;
			}
			return null;
		}

		public int? GetAuthenticatedUserId(ClaimsPrincipal User)
		{
			var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (!int.TryParse(userIdString, out var userId))
			{
				return null;
			}
			return userId;
		}

		public async Task<string> CreateJwtToken(ApplicationUser user)
		{
			var tokenExpirationDays = _configuration.GetValue("Spark:Jwt:ExpirationDays", 5);
			var expiration = DateTime.UtcNow.AddDays(tokenExpirationDays);
			var token = CreateJwtSecurityToken(
				await CreateJwtClaimsAsync(user),
				CreateJwtSigningCredentials(),
				expiration
			);
			var tokenHandler = new JwtSecurityTokenHandler();
			return tokenHandler.WriteToken(token);
		}

		private JwtSecurityToken CreateJwtSecurityToken(List<Claim> claims, SigningCredentials credentials,
		DateTime expiration) =>
		new(
				_configuration.GetValue("Spark:Jwt:Issuer", "https://spark-framework.net"),
				_configuration.GetValue("Spark:Jwt:Audience", "https://spark-framework.net"),
				claims,
				expires: expiration,
				signingCredentials: credentials
			);

		private async Task<List<Claim>> CreateJwtClaimsAsync(ApplicationUser user)
		{
			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),

				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(CultureInfo.InvariantCulture)),
				new Claim(ClaimTypes.Email, user.Email?? ""),
				new Claim(ClaimTypes.Name, user.UserName ?? ""),
				new Claim(ClaimTypes.UserData, user.Id.ToString(CultureInfo.InvariantCulture))
			};

			// add roles
			var roles = await _userManager.GetRolesAsync(user);
			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
			}

			return claims;
		}
		private SigningCredentials CreateJwtSigningCredentials()
		{
			return new SigningCredentials(
				new SymmetricSecurityKey(
					Encoding.UTF8.GetBytes(_configuration.GetValue("Spark:Jwt:Key", "SomthingSecret!"))
				),
				SecurityAlgorithms.HmacSha256
			);
		}
	}
}
