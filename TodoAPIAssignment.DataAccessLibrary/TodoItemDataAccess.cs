using Microsoft.EntityFrameworkCore;
using TodoAPIAssignment.DataAccessLibrary.Enums;
using TodoAPIAssignment.DataAccessLibrary.Models;
using TodoAPIAssignment.DataAccessLibrary.Models.Results.TodoItemResults;
using TodoAPIAssignment.DataAccessLibrary.Models.Results.TodoResults;

namespace TodoAPIAssignment.DataAccessLibrary;

public class TodoItemDataAccess : ITodoItemDataAccess
{
    private readonly DataDbContext _dataDbContext;
    private readonly ITodoDataAccess _todoDataAccess;

    public TodoItemDataAccess(DataDbContext dataDbContext, ITodoDataAccess todoDataAccess)
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

    public async Task<GetTodoItemResult> GetUserTodoItemAsync(string userId, string todoId, string todoItemId)
    {
        try
        {
            Todo? foundTodo = await _dataDbContext.Todos.FirstOrDefaultAsync(todo => todo.UserId == userId && todo.Id == todoId);
            if (foundTodo is null)
                return new GetTodoItemResult() { ErrorCode = ErrorCode.TodoNotFound, TodoItem = null };

            TodoItem? foundTodoItem = foundTodo.TodoItems.FirstOrDefault(todoItem => todoItem.Id == todoItemId);
            if (foundTodoItem is null)
                return new GetTodoItemResult() { ErrorCode = ErrorCode.TodoItemNotFound, TodoItem = null };

            return new GetTodoItemResult() { ErrorCode = ErrorCode.None, TodoItem = foundTodoItem };
        }
        catch (Exception)
        {
            return new GetTodoItemResult() { ErrorCode = ErrorCode.DatabaseError, TodoItem = null };
        }
    }

    public async Task<UpdateTodoItemResult> UpdateUserTodoItemAsync(string userId, string todoId, TodoItem todoItem)
    {
        try
        {
            GetTodoItemResult getTodoItemResult = await GetUserTodoItemAsync(userId, todoId, todoItem.Id!);
            if (getTodoItemResult.ErrorCode == ErrorCode.TodoNotFound)
                return new UpdateTodoItemResult() { TodoItem = null, ErrorCode = ErrorCode.TodoNotFound };
            else if (getTodoItemResult.ErrorCode == ErrorCode.TodoItemNotFound)
                return new UpdateTodoItemResult() { TodoItem = null, ErrorCode = ErrorCode.TodoItemNotFound };
            else if (getTodoItemResult.ErrorCode == ErrorCode.DatabaseError)
                return new UpdateTodoItemResult() { TodoItem = null, ErrorCode = ErrorCode.DatabaseError };

            TodoItem foundTodoItem = getTodoItemResult.TodoItem!;
            foundTodoItem.Title = todoItem.Title;
            foundTodoItem.Description = todoItem.Description is not null ? todoItem.Description : foundTodoItem.Description;
            foundTodoItem.IsDone = todoItem.IsDone;

            await _dataDbContext.SaveChangesAsync();

            return new UpdateTodoItemResult() { ErrorCode = ErrorCode.None, TodoItem = foundTodoItem };
        }
        catch (Exception)
        {
            return new UpdateTodoItemResult() { ErrorCode = ErrorCode.DatabaseError, TodoItem = null };
        }
    }

    public async Task<ErrorCode> DeleteUserTodoItemAsync(string userId, string todoId, string todoItemId)
    {
        try
        {
            Todo? foundTodo = await _dataDbContext.Todos.FirstOrDefaultAsync(todo => todo.UserId == userId && todo.Id == todoId);
            if (foundTodo is null)
                return ErrorCode.TodoNotFound;

            TodoItem? foundTodoItem = foundTodo.TodoItems.FirstOrDefault(todoItem => todoItem.Id == todoItemId);
            if (foundTodoItem is null)
                return ErrorCode.TodoItemNotFound;

            foundTodo.TodoItems.Remove(foundTodoItem);
            await _dataDbContext.SaveChangesAsync();

            return ErrorCode.None;
        }
        catch (Exception)
        {
            return ErrorCode.DatabaseError;
        }
    }

}
