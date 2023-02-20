using CheckRuCode.Config;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace CheckRuCode.Service
{
    internal class MailSender
    {
        private string _HOST{get; set;}
        private int _PORT { get; set; }
        public MailSender(string host = "", int port = 000)             //change this
        {
            _HOST = host;
            _PORT = port;
        }

        public void SendEmail(string senderEmail, string sendToMail , string subject, string message, string attachmentFilePath)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(subject, senderEmail));
            emailMessage.To.Add(new MailboxAddress(subject, sendToMail));
            emailMessage.Subject = subject;

            var builder = new BodyBuilder();
            builder.Attachments.Add(Settings.CustomSettingsValues.OutputFileName);
            emailMessage.Body = builder.ToMessageBody();
            
            using (var client = new SmtpClient())
            {
                client.Connect(_HOST, _PORT, SecureSocketOptions.Auto);
                client.Authenticate("", "");                    //change this
                client.Send(emailMessage);
                client.Disconnect(true);
            }
        }
    }
}
