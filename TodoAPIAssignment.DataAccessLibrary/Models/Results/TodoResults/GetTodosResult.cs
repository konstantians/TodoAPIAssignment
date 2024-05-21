using TodoAPIAssignment.DataAccessLibrary.Enums;

namespace TodoAPIAssignment.DataAccessLibrary.Models.Results.TodoResults;

public class GetTodosResult
{
    public List<Todo>? Todos { get; set; }
    public ErrorCode ErrorCode { get; set; }
}
