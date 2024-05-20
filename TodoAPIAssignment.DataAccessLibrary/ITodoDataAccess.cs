using TodoAPIAssignment.DataAccessLibrary.Models;

namespace TodoAPIAssignment.DataAccessLibrary
{
    public interface ITodoDataAccess
    {
        Task<CreateTodoResult> CreateTodoAsync(Todo todo);
        Task<GetTodoResult> GetUserTodoAsync(string userId, string todoId);
        Task<GetTodosResult> GetUserTodosAsync(string userId);
        Task<UpdateTodoResult> UpdateUserTodoAsync(Todo updatedTodo);
    }
}