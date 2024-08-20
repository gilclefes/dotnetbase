using dotnetbase.Application.Models;
using Coravel.Events.Interfaces;

namespace dotnetbase.Application.Events
{

    public class UserEmailVerification : IEvent
    {
        public ApplicationUser User { get; set; }

        public string Token { get; set; }

        public UserEmailVerification(ApplicationUser user, string token)
        {
            this.User = user;
            this.Token = token;
        }
    }
}
