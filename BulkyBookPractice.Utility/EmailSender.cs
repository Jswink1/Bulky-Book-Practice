using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBookPractice.Utility
{
    public class EmailSender : IEmailSender
    {
        public EmailOptions Options { get; set; }

        public EmailSender(IOptions<EmailOptions> emailOptions)
        {
            Options = emailOptions.Value;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SendGridClient(Options.SendGridKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("jacobswink1@gmail.com", "BulkyBook"),
                Subject = subject,
                PlainTextContent = htmlMessage,
                HtmlContent = htmlMessage
            };
            msg.AddTo(new EmailAddress(email));

            try
            {
                return client.SendEmailAsync(msg);
            }
            catch (Exception)
            {

            }

            return null;
        }

        //public Task SendEmailAsync(string email, string subject, string htmlMessage)
        //{
        //    return Execute(emailOptions.SendGridKey, subject, htmlMessage, email);
        //}
        //private static Task Execute(string sendGridKey, string subject, string message, string email)
        //{
        //    var client = new SendGridClient(sendGridKey);
        //    var from = new EmailAddress("jacobswink1@gmail.com", "Bulky Books");
        //    var to = new EmailAddress(email, "End User");
        //    var msg = MailHelper.CreateSingleEmail(from, to, subject, "", message);
        //    return client.SendEmailAsync(msg);
        //}
    }
}
