namespace Habr.BusinessLogic.Validation;

public static class PageValidation
{
    public static bool IsPageValid(int elementCount, int pageSize, int pageNumber)
    {
        if (elementCount == 0)
        {
            return pageNumber == 1;
        }
        if (pageNumber < 1)
        {
            return false;
        }
        return elementCount + pageSize > pageSize * pageNumber; //count of pages less then requested page number
    }
}