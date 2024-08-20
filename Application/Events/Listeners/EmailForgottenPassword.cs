using dotnetbase.Application.Mail;
using Spark.Library.Mail;
using Coravel.Events.Interfaces;
using System.Web;

namespace dotnetbase.Application.Events.Listeners
{
    public class EmailForgottenPassword : IListener<UserForgottenPassword>
    {
        private readonly IMailer _mailer;
        private readonly IConfiguration _config;

        public EmailForgottenPassword(IMailer mailer, IConfiguration config)
        {
            this._mailer = mailer;
            _config = config;
        }

        public async Task HandleAsync(UserForgottenPassword broadcasted)
        {
            var user = broadcasted.User;
            var token = broadcasted.Token;
            var mail = new GenericMailable()
                .To(user.Email)
                .Subject($"Reset your password on {_config.GetValue<string>("APP_NAME")}")
                .Html($@"
<h1>Reset your password</h1>
<p>Click <a href='{_config.GetValue<string>("RESET_PASSWORD_LINK")}?token={HttpUtility.UrlEncode(token)}'>here</a> to reset your password.</p>
");
            await this._mailer.SendAsync(mail);
        }
    }
}
