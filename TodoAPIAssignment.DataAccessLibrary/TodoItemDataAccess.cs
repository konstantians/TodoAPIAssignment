using TodoAPIAssignment.DataAccessLibrary.Enums;
using TodoAPIAssignment.DataAccessLibrary.Models;
using TodoAPIAssignment.DataAccessLibrary.Models.Results.TodoItemResults;
using TodoAPIAssignment.DataAccessLibrary.Models.Results.TodoResults;

namespace TodoAPIAssignment.DataAccessLibrary;

public class TodoItemDataAccess
{
    private readonly DataDbContext _dataDbContext;
    private readonly TodoDataAccess _todoDataAccess;

    public TodoItemDataAccess(DataDbContext dataDbContext, TodoDataAccess todoDataAccess)
    {
        _dataDbContext = dataDbContext;
        _todoDataAccess = todoDataAccess;
    }

    public async Task<CreateTodoItemResult> CreateUserTodoItemAsync(string userId, string todoId, TodoItem todoItem)
    {
        try
        {
            GetTodoResult getTodoResult = await _todoDataAccess.GetUserTodoAsync(userId, todoId);
            if (getTodoResult.ErrorCode == ErrorCode.TodoNotFound)
                return new CreateTodoItemResult() { TodoItem = null, ErrorCode = ErrorCode.TodoNotFound };
            else if (getTodoResult.ErrorCode == ErrorCode.DatabaseError)
                return new CreateTodoItemResult() { TodoItem = null, ErrorCode = ErrorCode.DatabaseError };

            todoItem.Id = Guid.NewGuid().ToString();
            todoItem.CreatedAt = DateTime.Now;
            getTodoResult.Todo!.TodoItems.Add(todoItem);

            await _dataDbContext.SaveChangesAsync();

            return new CreateTodoItemResult() { ErrorCode = ErrorCode.None, TodoItem = todoItem };
        }
        catch (Exception)
        {
            return new CreateTodoItemResult() { ErrorCode = ErrorCode.DatabaseError, TodoItem = null };
        }
    }
}
