using Launcher.Infrastructure.Security.Models;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Launcher.Infrastructure.Security
{
    public class SMTPSender
    {
        private readonly IConfiguration Configuration;
        private readonly SMTPOptions account;

        public SMTPSender(IConfiguration configuration)
        {
            Configuration = configuration;

            account = new SMTPOptions();
            Configuration.GetSection("SMTPAuthorization").Bind(account);
        }

        public void SendConfirmationEmail(string mailTo, int code)
        {
            MailAddress from = new MailAddress(account.Email, account.Mail.DisplayName);
            MailAddress to = new MailAddress(mailTo);
            MailMessage m = new MailMessage(from, to);
            m.Subject = account.Mail.Header;
            m.Body = account.Mail.Body + code;
            m.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient(account.Host, account.Port)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(account.Email, account.Password),
                EnableSsl = true
            };

            try
            {
                smtp.Send(m);
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
    }
}
