namespace Habr.BusinessLogic.Validation;

public static class PostValidation
{
    public const int MaxTextLength = 2000;
    public const int MaxTitleLength = 200;

    public static bool TitleValidation(string? title)
    {
        return title is {Length: <= MaxTitleLength};
    }

    public static bool TextValidation(string? text)
    {
        return text is {Length: <= MaxTextLength};
    }
}