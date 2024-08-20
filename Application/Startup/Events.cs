using dotnetbase.Application.Events.Listeners;
using dotnetbase.Application.Events;
using Coravel.Events.Interfaces;
using Coravel;

namespace dotnetbase.Application.Startup
{
    public static class Events
    {
        public static IServiceProvider RegisterEvents(this IServiceProvider services)
        {
            IEventRegistration registration = services.ConfigureEvents();

            // add events and listeners here
            registration.Register<UserCreated>().Subscribe<EmailNewUser>();

            registration.Register<OrderAssignedEvent>().Subscribe<EmailOrderAssigned>();
            registration.Register<OrderMessageEvent>().Subscribe<EmailOrderMessage>();
            registration.Register<UserForgottenPassword>().Subscribe<EmailForgottenPassword>();

            registration.Register<UserEmailVerification>().Subscribe<EmailCodeConfirmation>();

            return services;
        }
    }
}
