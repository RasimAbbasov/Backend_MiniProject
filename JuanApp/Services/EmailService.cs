using JuanApp.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace JuanApp.Services
{
    public class EmailService
    {
        public void SendEmail(string to, string subject, string body, EmailSetting emailSetting)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(emailSetting.FromEmail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };
            using var smtp = new SmtpClient();
            smtp.Connect(emailSetting.SmtpServer, emailSetting.SmtpPort, SecureSocketOptions.StartTls);
            smtp.Authenticate(emailSetting.FromEmail, emailSetting.SmtpPass);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
        public void SendEmails(List<string> to, string subject, string body, EmailSetting emailSetting)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(emailSetting.FromEmail));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };
            using var smtp = new SmtpClient();

            smtp.Connect(emailSetting.SmtpServer, emailSetting.SmtpPort, SecureSocketOptions.StartTls);
            smtp.Authenticate(emailSetting.FromEmail, emailSetting.SmtpPass);

            foreach (var item in to)
            {
                email.To.Clear(); 
                email.To.Add(MailboxAddress.Parse(item));
                smtp.Send(email);
            }
            smtp.Disconnect(true);
        }
    }
}
