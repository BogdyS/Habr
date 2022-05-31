using FluentValidation;
using Habr.Common.DTO;

namespace Habr.BusinessLogic.Validation;

public class PostValidator : AbstractValidator<IPost>
{
    public const int MaxTextLength = 2000;
    public const int MaxTitleLength = 200;

    public PostValidator()
    {
        RuleFor(post => post.Title)
            .NotEmpty()
            .WithMessage("The Title is required");
        RuleFor(post => post.Title)
            .MaximumLength(MaxTitleLength)
            .WithMessage($"The Title must be less than {MaxTitleLength} symbols");

        RuleFor(post => post.Text)
            .NotEmpty()
            .WithMessage("The Text is required");
        RuleFor(post => post.Text)
            .MaximumLength(MaxTextLength)
            .WithMessage($"The Text must be less than {MaxTextLength} symbols");


    }
}