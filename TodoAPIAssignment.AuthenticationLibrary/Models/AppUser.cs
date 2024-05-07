namespace TodoAPIAssignment.AuthenticationLibrary.Models;

public class AppUser
{
    public string? Id { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
    public int AccessTokenCountId { get; set; }
}
