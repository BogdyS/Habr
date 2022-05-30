using Habr.Common.DTO;

namespace Habr.BusinessLogic.Helpers;

public static class ExceptionParser
{
    public static ExceptionDTO ToDto(this Exception exception)
    {
        return new ExceptionDTO() { Message = exception.Message };
    }
}