using FluentValidation;
using Habr.Common.DTO;
using Habr.Common.Resourses;

namespace Habr.BusinessLogic.Validation;

public class PostValidator : AbstractValidator<IPostDTO>
{
    public const int MaxTextLength = 2000;
    public const int MaxTitleLength = 200;

    public PostValidator()
    {
        RuleFor(post => post.Title)
            .NotEmpty()
            .WithMessage(ExceptionMessages.TitleRequired);
        RuleFor(post => post.Title)
            .MaximumLength(MaxTitleLength)
            .WithMessage(string.Format(ExceptionMessages.TitleOverLimit, MaxTitleLength));

        RuleFor(post => post.Text)
            .NotEmpty()
            .WithMessage(ExceptionMessages.TextRequired);
        RuleFor(post => post.Text)
            .MaximumLength(MaxTextLength)
            .WithMessage(string.Format(ExceptionMessages.TextOverLimit, MaxTextLength));


    }
}