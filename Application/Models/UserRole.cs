namespace dotnetbase.Application.Models
{
    public class UserRole
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Role Role { get; set; }
    }
}
