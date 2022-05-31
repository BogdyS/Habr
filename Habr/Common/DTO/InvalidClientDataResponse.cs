namespace Habr.Common.DTO;

public class InvalidClientDataResponse
{
    public InvalidClientDataResponse(int code, string message, string value)
    {
        Code = code;
        Message = message;
        Value = value;
    }

    public int Code { get; set; }
    public string Message { get; set; }
    public string Value { get; set; }
}