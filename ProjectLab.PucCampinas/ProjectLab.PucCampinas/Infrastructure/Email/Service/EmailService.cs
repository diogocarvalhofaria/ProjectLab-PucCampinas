using HandlebarsDotNet;
using System.Net;
using System.Net.Mail;
using System.Reflection;

namespace ProjectLab.PucCampinas.Infrastructure.Email.Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendTemplateEmail<T>(string toEmail, string subject, string templateName, T model)
        {
            var host = _configuration["EmailSettings:Host"];
            var port = int.Parse(_configuration["EmailSettings:Port"]);
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderName = _configuration["EmailSettings:SenderName"];
            var password = _configuration["EmailSettings:Password"];

            var executePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var templatePath = Path.Combine(executePath!, "Infrastructure", "Email", "Templates", $"{templateName}.hbs");

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Template de email não encontrado: {templatePath}");

            var templateSource = await File.ReadAllTextAsync(templatePath);

            var template = Handlebars.Compile(templateSource);
            var bodyHtml = template(model); 

            var smtpClient = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(senderEmail, password),
                EnableSsl = true
            };

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
    }
}
