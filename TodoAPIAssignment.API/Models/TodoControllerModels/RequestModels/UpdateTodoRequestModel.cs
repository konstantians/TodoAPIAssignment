using System.ComponentModel.DataAnnotations;

namespace TodoAPIAssignment.API.Models.TodoControllerModels.RequestModels;

public class UpdateTodoRequestModel
{
    [Required]
    public string? Id { get; set; }
    public string? Title { get; set; }
    public bool IsDone { get; set; }
}
