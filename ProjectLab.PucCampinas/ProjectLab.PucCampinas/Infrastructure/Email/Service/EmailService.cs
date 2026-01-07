using HandlebarsDotNet;
using ProjectLab.PucCampinas.Common.Services;
using System.Net;
using System.Net.Mail;
using System.Reflection;

namespace ProjectLab.PucCampinas.Infrastructure.Email.Service
{
    public class EmailService : BaseService, IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration, ICustomErrorHandler errorHandler)
            : base(errorHandler)
        {
            _configuration = configuration;
        }

        public async Task SendTemplateEmail<T>(string toEmail, string subject, string templateName, T model)
        {
            try
            {
                var host = _configuration["EmailSettings:Host"];
                var port = int.Parse(_configuration["EmailSettings:Port"]);
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderName = _configuration["EmailSettings:SenderName"];
                var userName = _configuration["EmailSettings:UserName"];
                var password = _configuration["EmailSettings:Password"];

                var executePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var templatePath = Path.Combine(executePath!, "Infrastructure", "Email", "Templates", $"{templateName}.hbs");

                if (!File.Exists(templatePath))
                    throw new FileNotFoundException($"Template de email não encontrado: {templatePath}");

                var templateSource = await File.ReadAllTextAsync(templatePath);

                var template = Handlebars.Compile(templateSource);
                var bodyHtml = template(model);

                using var smtpClient = new SmtpClient(host, port);

                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(userName, password);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = subject,
                    Body = bodyHtml,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                OnError(ex, 500);
                throw;
            }
        }
    }
}