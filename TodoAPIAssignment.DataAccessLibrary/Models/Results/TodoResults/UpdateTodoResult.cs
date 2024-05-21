using TodoAPIAssignment.DataAccessLibrary.Enums;

namespace TodoAPIAssignment.DataAccessLibrary.Models.Results.TodoResults;

public class UpdateTodoResult
{
    public Todo? Todo { get; set; }
    public ErrorCode ErrorCode { get; set; }
}
