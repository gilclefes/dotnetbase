using dotnetbase.Application.Mail;
using Spark.Library.Mail;
using Coravel.Events.Interfaces;

namespace dotnetbase.Application.Events.Listeners
{
    public class EmailOrderMessage : IListener<OrderMessageEvent>
    {
        private readonly IMailer _mailer;
        private readonly IConfiguration _config;

        public EmailOrderMessage(IMailer mailer, IConfiguration config)
        {
            this._mailer = mailer;
            _config = config;
        }

        public async Task HandleAsync(OrderMessageEvent broadcasted)
        {
            var mail = new GenericMailable()
                .To(broadcasted.UserEmail)
                .Subject($"Received Order Message on {_config.GetValue<string>("APP_NAME")}")
                .Html($@"
        <h1>Messag receive on Order: {broadcasted.OrderRefNumber}</h1>
        <p> {broadcasted.Message} </p>
        
        ");
            await this._mailer.SendAsync(mail);
        }
    }
}
