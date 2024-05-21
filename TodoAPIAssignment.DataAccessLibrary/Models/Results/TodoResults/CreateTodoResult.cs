using TodoAPIAssignment.DataAccessLibrary.Enums;

namespace TodoAPIAssignment.DataAccessLibrary.Models.Results.TodoResults;

public class CreateTodoResult
{
    public Todo? Todo { get; set; }
    public ErrorCode ErrorCode { get; set; }
}
