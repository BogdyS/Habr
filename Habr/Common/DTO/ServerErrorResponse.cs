namespace Habr.Common.DTO;

public class ServerErrorResponse
{
    public ServerErrorResponse(int code, string message, string stackTrace)
    {
        Code = code;
        Message = message;
        StackTrace = stackTrace;
    }

    public int Code { get; set; }
    public string Message { get; set; }
    public string StackTrace { get; set; }
}