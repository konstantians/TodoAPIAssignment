using TodoAPIAssignment.DataAccessLibrary.Enums;
using TodoAPIAssignment.DataAccessLibrary.Models;
using TodoAPIAssignment.DataAccessLibrary.Models.Results.TodoItemResults;

namespace TodoAPIAssignment.DataAccessLibrary
{
    public interface ITodoItemDataAccess
    {
        Task<CreateTodoItemResult> CreateUserTodoItemAsync(string userId, string todoId, TodoItem todoItem);
        Task<ErrorCode> DeleteUserTodoItemAsync(string userId, string todoId, string todoItemId);
        Task<GetTodoItemResult> GetUserTodoItemAsync(string userId, string todoId, string todoItemId);
        Task<UpdateTodoItemResult> UpdateUserTodoItemAsync(string userId, string todoId, TodoItem todoItem);
    }
}