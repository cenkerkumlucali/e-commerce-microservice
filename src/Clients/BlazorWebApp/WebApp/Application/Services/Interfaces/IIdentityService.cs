namespace WebApp.Application.Services.Interfaces;

public interface IIdentityService
{
    string GetUserName();
    string GetUserToken();
    bool IsLoggedIn { get; }
    Task<bool> Login(string userName, string password);
    void Logout();
}