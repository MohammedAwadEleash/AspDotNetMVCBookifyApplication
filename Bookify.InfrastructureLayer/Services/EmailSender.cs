using System.Net;
using System.Net.Mail;

namespace Bookify.Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {

        private readonly MailSettings _mailSettings;
        private readonly IAppEnvironmentService _appEnvironmentService;

        public EmailSender(IOptions<MailSettings> mailSettings, IAppEnvironmentService appEnvironmentService)
        {
            _mailSettings = mailSettings.Value;
            _appEnvironmentService = appEnvironmentService;
        }



        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            MailMessage message = new()
            {

                From = new MailAddress(_mailSettings.Email!, _mailSettings.DisplayName),
                Body = htmlMessage,
                Subject = subject,
                IsBodyHtml = true




            };


            message.To.Add(_appEnvironmentService.IsDevelopment ? "MohammedAwad700@outlook.com" : email);
            var smtpClient = new SmtpClient(_mailSettings.Host)
            {

                Port = _mailSettings.Port,
                Credentials = new NetworkCredential(_mailSettings.Email, _mailSettings.Password),
                EnableSsl = true,



            };


            await smtpClient.SendMailAsync(message);
            smtpClient.Dispose();

        }

    }
}
