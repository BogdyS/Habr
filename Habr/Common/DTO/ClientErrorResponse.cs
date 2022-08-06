namespace Habr.Common.DTO;

public class ClientErrorResponse
{
    public ClientErrorResponse(int code, string message)
    {
        Code = code;
        Message = message;
    }
    public int Code { get; set; }
    public string Message { get; set; }
}