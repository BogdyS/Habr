using AutoMapper;
using AutoMapper.QueryableExtensions;
using Habr.Common.DTO.User;
using MimeKit;
using Habr.Common.Resourses;
using Habr.DataAccess;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using MimeKit.Text;

namespace Habr.WebAPI.BackgroundJobs;

public class BirthdayTask : IEmailSender
{
    private readonly DataContext _dataContext;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public BirthdayTask(DataContext dataContext, IConfiguration configuration, IMapper mapper)
    {
        _dataContext = dataContext;
        _configuration = configuration;
        _mapper = mapper;
    }

    public async Task SendEmailAsync()
    {
        var today = DateTime.UtcNow;

        var todayBirthdays = await _dataContext
            .Users
            .Where(u => u.DateOfBirth.Day == today.Day)
            .Where(u => u.DateOfBirth.Month == today.Month)
            .ProjectTo<BirthdayDTO>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .ToListAsync();

        string password = _configuration["Email:Password"];
        string emailFrom = _configuration["Email:EmailAddress"];

        using var client = new SmtpClient();
        await client.ConnectAsync("smtp.mail.ru", 25, false);
        await client.AuthenticateAsync(emailFrom, password);

        foreach (var user in todayBirthdays)
        {
            var message = new MimeMessage();
            message.Subject = string.Format(Email.BirthdayTitle, user.Name);
            message.Body = new TextPart(TextFormat.Plain)
            {
                Text = string.Format(Email.BirthdayText, user.Name)
            };
            message.From.Add(new MailboxAddress(Email.Name, emailFrom));
            message.To.Add(new MailboxAddress("", user.Email));
            await client.SendAsync(message);
        }
        await client.DisconnectAsync(true);
    }
}

