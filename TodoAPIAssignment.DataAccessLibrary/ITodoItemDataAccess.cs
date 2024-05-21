using TodoAPIAssignment.DataAccessLibrary.Models;
using TodoAPIAssignment.DataAccessLibrary.Models.Results.TodoItemResults;

namespace TodoAPIAssignment.DataAccessLibrary
{
    public interface ITodoItemDataAccess
    {
        Task<CreateTodoItemResult> CreateUserTodoItemAsync(string userId, string todoId, TodoItem todoItem);
    }
}