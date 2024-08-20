using dotnetbase.Application.Models;
using Coravel.Events.Interfaces;

namespace dotnetbase.Application.Events
{

    public class UserCreated : IEvent
    {
        public ApplicationUser User { get; set; }

        public UserCreated(ApplicationUser user)
        {
            this.User = user;
        }
    }
}
