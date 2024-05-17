using System.ComponentModel.DataAnnotations;

namespace TodoAPIAssignment.API.Models.AuthenticationControllerModels.RequestModels;

public class LogOutRequestModel
{
    [Required]
    public string? Token { get; set; }
}
