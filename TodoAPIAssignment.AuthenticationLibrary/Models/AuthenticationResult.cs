using TodoAPIAssignment.AuthenticationLibrary.Enums;

namespace TodoAPIAssignment.AuthenticationLibrary.Models;

public class AuthenticationResult
{
    public string? Token { get; set; }
    public ErrorCode ErrorCode { get; set; }
}
