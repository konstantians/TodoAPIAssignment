using System.ComponentModel.DataAnnotations;

namespace TodoAPIAssignment.API.Models.RequestModels;

public class LogInRequestModel
{
    [Required]
    public string? Username { get; set; }
    [Required]
    public string? Password { get; set; }
}
