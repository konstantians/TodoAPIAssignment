using TodoAPIAssignment.AuthenticationLibrary.Enums;
using TodoAPIAssignment.AuthenticationLibrary.Models;

namespace TodoAPIAssignment.AuthenticationLibrary;

public interface IAuthenticationDataAccess
{
    Task<AppUser?> CheckAndDecodeAccessTokenAsync(string accessToken);
    Task<AuthenticationResult>? LogInAsync(string username, string password);
    Task<ErrorCode> LogOutAsync(string accessToken);
    Task<AuthenticationResult>? SignUpAsync(string username, string password, string email);
}