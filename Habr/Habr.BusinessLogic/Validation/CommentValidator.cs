using FluentValidation;
using Habr.Common.DTO;
using Habr.Common.Resourses;

namespace Habr.BusinessLogic.Validation;

public class CommentValidator : AbstractValidator<CreateCommentDTO>
{
    public const int MaxTextLength = 200;
    public CommentValidator()
    {
        RuleFor(comment => comment.Text)
            .NotEmpty()
            .WithMessage(ExceptionMessages.TextRequired);
        RuleFor(comment => comment.Text)
            .MaximumLength(MaxTextLength)
            .WithMessage(string.Format(ExceptionMessages.TextOverLimit, MaxTextLength));
    }
}