using System.ComponentModel.DataAnnotations;

namespace TodoAPIAssignment.API.Models.TodoControllerModels.RequestModels;

public class CreateTodoRequestModel
{
    [Required]
    public string? Title { get; set; }
    public bool IsDone { get; set; }
    [Required]
    public string? Token { get; set; }
}
