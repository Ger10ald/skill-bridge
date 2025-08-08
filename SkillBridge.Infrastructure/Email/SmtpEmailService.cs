using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Application.Interfaces;


namespace SkillBridge.Infrastructure.Email
{
    public class SmtpEmailService : IEmailService
    {
        public async Task SendEmailAsync(string to, string subject, string body) 
        {
            using var client = new SmtpClient("smtp.example.com")
            {

            };

            var message = new MailMessage("no-reply@skillbridge.com", to, subject, body);
            await client.SendMailAsync(message);
        }
    }
}
