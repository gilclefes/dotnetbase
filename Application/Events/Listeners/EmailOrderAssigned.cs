using dotnetbase.Application.Mail;
using Spark.Library.Mail;
using Coravel.Events.Interfaces;

namespace dotnetbase.Application.Events.Listeners
{
    public class EmailOrderAssigned : IListener<OrderAssignedEvent>
    {
        private readonly IMailer _mailer;
        private readonly IConfiguration _config;

        public EmailOrderAssigned(IMailer mailer, IConfiguration config)
        {
            this._mailer = mailer;
            _config = config;
        }

        public async Task HandleAsync(OrderAssignedEvent broadcasted)
        {
            var mail = new GenericMailable()
                .To(broadcasted.UserEmail)
                .Subject($"Accept your Order on {_config.GetValue<string>("APP_NAME")}")
                .Html($@"
        <h1>Order Confirmation for {broadcasted.OrderRefNumber}</h1>
        <p>Confirm this Order {broadcasted.OrderRefNumber} by logging into your app. </p>
        <p>You have 30 minutes to confirm this order else it will eb reassigned.</p>
        ");
            await this._mailer.SendAsync(mail);
        }
    }
}
