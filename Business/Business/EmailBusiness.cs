using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Core.Business
{
    public class EmailBusiness : IEmailBusiness
    {
        private readonly IConfiguration _config;

        public EmailBusiness(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(string to, string subject, string body, bool isHtml = true)
        {
            var host = _config["SmtpSettings:Host"];
            var port = int.Parse(_config["SmtpSettings:Port"]);
            var username = _config["SmtpSettings:Username"];
            var password = _config["SmtpSettings:Password"];
            var from = _config["SmtpSettings:From"];

            try 
            {
                using var smtp = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true
                };

                var mail = new MailMessage(from, to, subject, body)
                {
                    IsBodyHtml = isHtml
                };

                await smtp.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                // Mostralo en la consola o loguealo para ver qué falla
                Console.WriteLine("❌ Error SMTP: " + ex.Message);
                throw; // Dejalo para que suba el error al frontend
            }


        }
    }
}