namespace TodoAPIAssignment.AuthenticationLibrary.Enums;

public enum ErrorCode
{
    None = 0,
    DatabaseError = 1,
    DuplicateUsername = 2,
    DuplicateEmail = 3,
    InvalidCredentials = 4,
    InvalidAccessToken = 5
}
