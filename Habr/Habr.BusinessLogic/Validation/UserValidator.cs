using FluentValidation;
using Habr.Common.DTO.User;
using Habr.Common.Resourses;

namespace Habr.BusinessLogic.Validation;

public class UserValidator : AbstractValidator<RegistrationDTO>
{
    public const int MaximumNameLength = 60;
    public const int MaximumLoginLength = 200;
    public const int MinimumPasswordLength = 6;
    public const int MaximumPasswordLength = 50;
    public UserValidator()
    {
        RuleFor(user => user.Name)
            .NotEmpty()
            .WithMessage(ExceptionMessages.NameRequired);
        RuleFor(user => user.Name)
            .MaximumLength(MaximumNameLength)
            .WithMessage(ExceptionMessages.NameOverLimit);

        RuleFor(user => user.Login)
            .NotEmpty()
            .WithMessage(ExceptionMessages.LoginRequired);
        RuleFor(user => user.Login)
            .MaximumLength(MaximumLoginLength)
            .WithMessage(ExceptionMessages.LoginOverLimit);
        RuleFor(user => user.Login)
            .EmailAddress()
            .WithMessage(ExceptionMessages.InvalidEmail);

        RuleFor(user => user.Password)
            .NotEmpty()
            .WithMessage(ExceptionMessages.PasswordRequired);
        RuleFor(user => user.Password)
            .Must(password => password!.ToUpper() != password!.ToLower())
            .MinimumLength(MinimumPasswordLength)
            .WithMessage(ExceptionMessages.PasswordNotSecure);
        RuleFor(user => user.Password)
            .MaximumLength(MaximumPasswordLength)
            .WithMessage(ExceptionMessages.PasswordOverLimit);

        RuleFor(user => user.DateOfBirth)
            .LessThan(Convert.ToDateTime(DateTime.UtcNow - new DateTime(6, 0, 0)))
            .WithMessage(ExceptionMessages.InvalidDateOfBirth);
    }
}