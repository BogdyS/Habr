using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Habr.Common.DTO.User;

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
            .WithMessage("Name is required");
        RuleFor(user => user.Name)
            .MaximumLength(MaximumNameLength)
            .WithMessage($"Name's length must be less than {MaximumNameLength} symbols");

        RuleFor(user => user.Login)
            .NotEmpty()
            .WithMessage("Login is required");
        RuleFor(user => user.Login)
            .MaximumLength(MaximumLoginLength)
            .WithMessage($"Login's length must be less than {MaximumLoginLength} symbols");
        RuleFor(user => user.Login)
            .EmailAddress()
            .WithMessage($"Invalid Email");

        RuleFor(user => user.Password)
            .NotEmpty()
            .WithMessage("Password is required");
        RuleFor(user => user.Password)
            .Must(password => password!.ToUpper() != password!.ToLower())
            .MinimumLength(MinimumPasswordLength)
            .WithMessage("Password is not secure");
        RuleFor(user => user.Password)
            .MaximumLength(MaximumPasswordLength)
            .WithMessage($"Password's length must be less than {MaximumPasswordLength}");
    }
}