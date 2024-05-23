using System.ComponentModel.DataAnnotations;

namespace TodoAPIAssignment.API.Models.TodoItemsControllerModels.RequestModels;

public class UpdateTodoItemRequestModel
{
    [Required]
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool IsDone { get; set; }
}
