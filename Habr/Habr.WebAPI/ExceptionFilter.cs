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

    public override async Task OnExceptionAsync(ExceptionContext context)
    {
        if (context.Exception is InputException inputException)
        {
            context.HttpContext.Response.StatusCode = inputException.Code;
            context.Result = new JsonResult(
                new InvalidClientDataResponse(inputException.Code, inputException.Message, inputException.Value!));
        }
        else if (context.Exception is BaseException baseException)
        {
            context.HttpContext.Response.StatusCode = baseException.Code;
            context.Result = new JsonResult(
                new ClientErrorResponse(baseException.Code, baseException.Message));
        }
        else
        {
            var exception = context.Exception;
            context.HttpContext.Response.StatusCode = 500;
            context.Result = new JsonResult(
                new ServerErrorResponse(500, exception.Message, exception.StackTrace!));
        }

        await base.OnExceptionAsync(context);
    }
}