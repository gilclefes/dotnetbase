using System.ComponentModel.DataAnnotations;

namespace dotnetbase.Application.ViewModels
{
    public class Register
    {
        [Required(ErrorMessage = "Fullname is required")]
        public required string Name { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Required(ErrorMessage = "Email is required")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }

        // public bool? IsProvider { get; set; }
    }
}
