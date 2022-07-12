using FluentValidation;
using Habr.Common.Resourses;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Validation;

public class RateValidator :AbstractValidator<Rate>
{
    public const int MaximumRate = 5;
    public const int MinimumRate = 1;

    public RateValidator()
    {
        RuleFor(r => r.Value)
            .LessThanOrEqualTo(5)
            .WithMessage(string.Format(ExceptionMessages.RateTooBig, MaximumRate.ToString()));

        RuleFor(r => r.Value)
            .GreaterThanOrEqualTo(MinimumRate)
            .WithMessage(string.Format(ExceptionMessages.RateTooSmall, MinimumRate.ToString()));
    }
}