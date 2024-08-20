using dotnetbase.Application.Models;
using Coravel.Events.Interfaces;

namespace dotnetbase.Application.Events
{

    public class UserForgottenPassword : IEvent
    {
        public ApplicationUser User { get; set; }
        public string Token { get; set; }

        public UserForgottenPassword(ApplicationUser user, string token)
        {
            this.User = user;
            this.Token = token;
        }
    }
}
