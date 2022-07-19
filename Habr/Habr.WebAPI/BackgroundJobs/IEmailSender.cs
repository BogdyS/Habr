using Habr.WebAPI.BackgroundJobs.Context;

namespace Habr.WebAPI.BackgroundJobs;

public interface IEmailSender
{
    Task SendEmailAsync();
}