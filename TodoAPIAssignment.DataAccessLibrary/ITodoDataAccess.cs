using TodoAPIAssignment.DataAccessLibrary.Enums;
using TodoAPIAssignment.DataAccessLibrary.Models;
using TodoAPIAssignment.DataAccessLibrary.Models.Results.TodoResults;

namespace TodoAPIAssignment.DataAccessLibrary
{
    public interface ITodoDataAccess
    {
        Task<CreateTodoResult> CreateTodoAsync(Todo todo);
        Task<ErrorCode> DeleteUserTodoAsync(string userId, string todoId);
        Task<GetTodoResult> GetUserTodoAsync(string userId, string todoId);
        Task<GetTodosResult> GetUserTodosAsync(string userId);
        Task<UpdateTodoResult> UpdateUserTodoAsync(Todo updatedTodo);
    }
}