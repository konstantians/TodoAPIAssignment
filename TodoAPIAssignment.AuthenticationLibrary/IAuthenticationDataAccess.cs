using TodoAPIAssignment.AuthenticationLibrary.Models;

namespace TodoAPIAssignment.AuthenticationLibrary;

public interface IAuthenticationDataAccess
{
    Task<AuthenticationResult>? LogInAsync(string username, string password);
    Task<AuthenticationResult>? SignUpAsync(string username, string password, string email);
}