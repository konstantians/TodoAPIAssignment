
namespace TodoAPIAssignment.AuthenticationLibrary;

public interface IAuthenticationDataAccess
{
    Task<string>? SignUpAsync(string username, string password, string email);
}