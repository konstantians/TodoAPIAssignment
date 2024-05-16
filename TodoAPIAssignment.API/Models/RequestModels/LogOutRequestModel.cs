using System.ComponentModel.DataAnnotations;

namespace TodoAPIAssignment.API.Models.RequestModels;

public class LogOutRequestModel
{
    [Required]
    public string? Token { get; set; }
}
