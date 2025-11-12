
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;

namespace Cinema_Ticket.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("wm1336562@gmail.com", "scsq bocr sypf gojp")
            };
            return client.SendMailAsync(new MailMessage
                (
                from: "wm1336562@gmail.com",
                to: email,
                subject,
                htmlMessage
                )
            {
                IsBodyHtml = true 
            });
        }
    }
}
