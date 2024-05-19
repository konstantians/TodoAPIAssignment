using TodoAPIAssignment.DataAccessLibrary.Enums;

namespace TodoAPIAssignment.DataAccessLibrary.Models;

public class GetTodoResult
{
    public Todo? Todo { get; set; }
    public ErrorCode ErrorCode { get; set; }
}
