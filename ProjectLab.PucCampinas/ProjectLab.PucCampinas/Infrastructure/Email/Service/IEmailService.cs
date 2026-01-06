namespace ProjectLab.PucCampinas.Infrastructure.Email.Service
{
    public interface IEmailService
    {
        Task SendTemplateEmail<T>(string toEmail, string subject, string templateName, T model);
    }
}
