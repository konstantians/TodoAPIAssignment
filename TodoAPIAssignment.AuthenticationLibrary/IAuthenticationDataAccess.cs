
namespace TodoAPIAssignment.AuthenticationLibrary;

public interface IAuthenticationDataAccess
{
    Task<string>? LogInAsync(string username, string password);
    Task<string>? SignUpAsync(string username, string password, string email);
}