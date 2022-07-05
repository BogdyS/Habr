namespace Habr.BusinessLogic.Validation;

public static class PageValidation
{
    public static bool IsPageValid(int elementCount, int pageSize, int pageNumber)
    {
        return elementCount + pageSize > pageSize * pageNumber; //count of pages less then requested page number
    }
}