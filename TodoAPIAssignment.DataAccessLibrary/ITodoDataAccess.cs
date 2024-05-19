using TodoAPIAssignment.DataAccessLibrary.Models;

namespace TodoAPIAssignment.DataAccessLibrary
{
    public interface ITodoDataAccess
    {
        Task<CreateTodoResult> CreateTodoAsync(Todo todo);
        Task<GetTodosResult> GetUserTodosAsync(string userId);
    }
}