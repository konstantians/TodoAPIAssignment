using System.ComponentModel.DataAnnotations;

namespace TodoAPIAssignment.API.Models.RequestModels;

public class SignUpRequestModel
{
    [Required]
    public string? Username { get; set; }
    [Required]
    public string? Password { get; set; }
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
}
