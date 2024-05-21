using TodoAPIAssignment.DataAccessLibrary.Enums;

namespace TodoAPIAssignment.DataAccessLibrary.Models.Results.TodoItemResults;

public class CreateTodoItemResult
{
    public TodoItem? TodoItem { get; set; }
    public ErrorCode ErrorCode { get; set; }
}
