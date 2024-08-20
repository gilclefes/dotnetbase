using dotnetbase.Application.Models;
using Coravel.Events.Interfaces;

namespace dotnetbase.Application.Events
{

    public class OrderAssignedEvent : IEvent
    {

        public string OrderRefNumber { get; set; }
        public string UserEmail { get; set; }

        public OrderAssignedEvent(string orderRefNumber, string userEmail)
        {
            this.OrderRefNumber = orderRefNumber;
            this.UserEmail = userEmail;

        }
    }
}
