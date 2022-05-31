using Habr.Common.DTO;
using Habr.Common.Exceptions;
using Habr.Common.Exceptions.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Habr.WebAPI;

public class ExceptionFilter : ExceptionFilterAttribute
{

    public ExceptionFilter()
    {
        
    }
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is InputException inputException)
        {
            context.HttpContext.Response.StatusCode = inputException.Code;
            context.Result = new JsonResult(inputException);
        }
        else if (context.Exception is BaseException baseException)
        {
            context.HttpContext.Response.StatusCode = baseException.Code;
            context.Result = new JsonResult(baseException);
        }
        else
        {
            var exception = context.Exception;
            context.HttpContext.Response.StatusCode = 500;
            context.Result = new JsonResult(new
                {Code = 500, Message = exception.Message, StackTrace = exception.StackTrace});
        }
    }
}