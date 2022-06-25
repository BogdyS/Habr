namespace Habr.BusinessLogic.Interfaces;

public interface IJwtService
{
    string GetJwt(object subObject);
    string GetRefreshToken();
}