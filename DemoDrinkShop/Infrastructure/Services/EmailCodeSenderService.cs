using System.Net;
using System.Net.Mail;
using DemoDrinkShop.Application.Interfaces;

namespace DemoDrinkShop.Infrastructure.Services
{
    public class EmailCodeSenderService : ICodeSenderService, IDisposable
    {
        private readonly string ourEmail;
        private readonly SmtpClient smtpClient;
        public EmailCodeSenderService()
        {
            string[] credentials = File.ReadAllLines("Credentials/email.txt");
            ourEmail = credentials[1];
            
            smtpClient = new SmtpClient()
            {
                Host = credentials[0],
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(ourEmail, credentials[2])
            };
        }

        public async Task SendCode(string emailTo, string code)
        {
            MailMessage mailMessage = new MailMessage()
            {
                From = new MailAddress(ourEmail),
                Subject = "Demo Drink Shop password change code",
                Body = $"{code}\nis your code that is valid for 15 minutes"
            };
            mailMessage.To.Add(new MailAddress(emailTo));
            
            using (mailMessage)
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(smtpClient);
            smtpClient.Dispose();
        }
    }
}
