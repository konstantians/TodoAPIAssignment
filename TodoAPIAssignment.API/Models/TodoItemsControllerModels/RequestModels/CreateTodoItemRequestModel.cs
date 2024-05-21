using System.ComponentModel.DataAnnotations;

namespace TodoAPIAssignment.API.Models.TodoItemsControllerModels.RequestModels;

public class CreateTodoItemRequestModel
{
    [Required]
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool IsDone { get; set; }

}
