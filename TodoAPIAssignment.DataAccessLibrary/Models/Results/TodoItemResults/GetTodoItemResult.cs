using TodoAPIAssignment.DataAccessLibrary.Enums;

namespace TodoAPIAssignment.DataAccessLibrary.Models.Results.TodoItemResults;

public class GetTodoItemResult
{
    public TodoItem? TodoItem { get; set; }
    public ErrorCode ErrorCode { get; set; }
}
