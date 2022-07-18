using MimeKit;
using Habr.Common.Resourses;
using Habr.WebAPI.BackgroundJobs.Context;
using MailKit.Net.Smtp;
using MimeKit.Text;

namespace Habr.WebAPI.BackgroundJobs;

public class BirthdayTask : IEmailSender
{
    public async Task SendEmailAsync(EmailContext context)
    {
        var message = new MimeMessage();
        message.Subject = string.Format(Email.BirthdayTitle, context.Name);
        message.Body = new TextPart(TextFormat.Plain)
        {
            Text = string.Format(Email.BirthdayText, context.Name)
        };

        message.From.Add(new MailboxAddress(Email.Name, context.EmailFrom));
        message.To.Add(new MailboxAddress("", context.EmailTo));

        using var client = new SmtpClient();
        await client.ConnectAsync("smtp.mail.ru", 25, true);
        await client.AuthenticateAsync(context.EmailFrom, context.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}

