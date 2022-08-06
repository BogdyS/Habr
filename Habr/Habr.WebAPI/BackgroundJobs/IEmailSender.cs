namespace Habr.WebAPI.BackgroundJobs;

public interface IEmailSender
{
    Task SendEmailAsync();
}