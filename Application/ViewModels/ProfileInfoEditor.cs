using System.ComponentModel.DataAnnotations;

namespace dotnetbase.Application.ViewModels
{
    public class ProfileInfoEditor
    {
        [Required]
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Required(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }
    }
}
