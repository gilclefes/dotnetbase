using dotnetbase.Application.Models;
using Coravel.Events.Interfaces;

namespace dotnetbase.Application.Events
{

    public class OrderMessageEvent : IEvent
    {

        public string OrderRefNumber { get; set; }
        public string UserEmail { get; set; }

        public string Message { get; set; }

        public OrderMessageEvent(string orderRefNumber, string userEmail, string message)
        {
            this.OrderRefNumber = orderRefNumber;
            this.UserEmail = userEmail;
            this.Message = message;

        }
    }
}
