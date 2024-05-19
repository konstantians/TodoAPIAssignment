using Microsoft.EntityFrameworkCore;
using TodoAPIAssignment.DataAccessLibrary.Enums;
using TodoAPIAssignment.DataAccessLibrary.Models;

namespace TodoAPIAssignment.DataAccessLibrary;

public class TodoDataAccess : ITodoDataAccess
{
    private readonly DataDbContext _dataDbContext;

    public TodoDataAccess(DataDbContext dataDbContext)
    {
        _dataDbContext = dataDbContext;
    }

    public async Task<CreateTodoResult> CreateTodoAsync(Todo todo)
    {
        try
        {
            todo.Id = Guid.NewGuid().ToString();
            todo.CreatedAt = DateTime.Now;
            await _dataDbContext.AddAsync(todo);

            await _dataDbContext.SaveChangesAsync();
            return new CreateTodoResult() { ErrorCode = ErrorCode.None, Todo = todo };
        }
        catch (Exception)
        {
            return new CreateTodoResult() { ErrorCode = ErrorCode.DatabaseError, Todo = null };
        }
    }

    public async Task<GetTodosResult> GetUserTodosAsync(string userId)
    {
        try
        {
           List<Todo> userTodos = await _dataDbContext.Todos.Where(todo => todo.UserId == userId).ToListAsync();
           return new GetTodosResult() { ErrorCode = ErrorCode.None, Todos = userTodos};
        }
        catch (Exception)
        {
            return new GetTodosResult() { ErrorCode = ErrorCode.DatabaseError, Todos = new List<Todo>() };
        }
    }
}
