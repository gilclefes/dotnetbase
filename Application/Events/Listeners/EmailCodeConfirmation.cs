using dotnetbase.Application.Mail;
using Spark.Library.Mail;
using Coravel.Events.Interfaces;
using System.Web;
using Microsoft.AspNetCore.Identity;
using dotnetbase.Application.Models;

namespace dotnetbase.Application.Events.Listeners
{
    public class EmailCodeConfirmation : IListener<UserEmailVerification>
    {
        private readonly IMailer _mailer;
        private readonly IConfiguration _config;


        public EmailCodeConfirmation(IMailer mailer, IConfiguration config)
        {
            this._mailer = mailer;
            _config = config;

        }

        public async Task HandleAsync(UserEmailVerification broadcasted)
        {
            var user = broadcasted.User;
            var token = broadcasted.Token;



            var mail = new GenericMailable()
                 .To(user.Email)
                 .Subject($"Confirm your Email on {_config.GetValue<string>("APP_NAME")}")
                 .Html($@"
        <h1>Email Confirmation</h1>
        <p>Confirm you email by clicking on the link below</p>
        <p>Click <a href='{_config.GetValue<string>("EMAIL_CONFIRMATION_LINK")}?userId={HttpUtility.UrlEncode(user.Id)}&token={HttpUtility.UrlEncode(token)}'>here</a> to confirm your email.</p>
        ");





            await this._mailer.SendAsync(mail);
        }
    }
}
