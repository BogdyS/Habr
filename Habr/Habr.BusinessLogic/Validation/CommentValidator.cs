using FluentValidation;
using Habr.Common.DTO;

namespace Habr.BusinessLogic.Validation;

public class CommentValidator : AbstractValidator<CreateCommentDTO>
{
    public const int MaxTextLength = 200;
    public CommentValidator()
    {
        RuleFor(comment => comment.Text)
            .NotEmpty()
            .WithMessage("The Text is required");
        RuleFor(comment => comment.Text)
            .MaximumLength(MaxTextLength)
            .WithMessage($"The Text must be less than {MaxTextLength} symbols");
    }
}