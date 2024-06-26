﻿using Microsoft.EntityFrameworkCore;
using TodoAPIAssignment.DataAccessLibrary.Enums;
using TodoAPIAssignment.DataAccessLibrary.Models;
using TodoAPIAssignment.DataAccessLibrary.Models.Results.TodoResults;

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
            await _dataDbContext.Todos.AddAsync(todo);

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

    public async Task<GetTodoResult> GetUserTodoAsync(string userId, string todoId)
    {
        try
        {
            Todo? foundTodo = await _dataDbContext.Todos.FirstOrDefaultAsync(todo => todo.UserId == userId && todo.Id == todoId);
            if (foundTodo is null)
                return new GetTodoResult() { ErrorCode = ErrorCode.TodoNotFound, Todo = null };
            
            return new GetTodoResult() { ErrorCode = ErrorCode.None, Todo = foundTodo };
        }
        catch (Exception)
        {
            return new GetTodoResult() { ErrorCode = ErrorCode.DatabaseError, Todo = null };
        }
    }

    public async Task<UpdateTodoResult> UpdateUserTodoAsync(Todo updatedTodo)
    {
        try
        {
            Todo? foundTodo = await _dataDbContext.Todos.FirstOrDefaultAsync(todo => todo.UserId == updatedTodo.UserId && todo.Id == updatedTodo.Id);
            if(foundTodo is null)
                return new UpdateTodoResult() { ErrorCode = ErrorCode.TodoNotFound, Todo = null };

            foundTodo.Title = updatedTodo.Title;
            foundTodo.IsDone = updatedTodo.IsDone;
            await _dataDbContext.SaveChangesAsync();

            return new UpdateTodoResult() { ErrorCode = ErrorCode.None, Todo = foundTodo };
        }
        catch (Exception)
        {
            return new UpdateTodoResult() { ErrorCode = ErrorCode.DatabaseError, Todo = null };
        }
    }

    public async Task<ErrorCode> DeleteUserTodoAsync(string userId, string todoId)
    {
        try
        {
            Todo? foundTodo = await _dataDbContext.Todos.FirstOrDefaultAsync(todo => todo.UserId == userId && todo.Id == todoId);
            if (foundTodo is null)
                return ErrorCode.TodoNotFound;

            _dataDbContext.Todos.Remove(foundTodo);
            await _dataDbContext.SaveChangesAsync();

            return ErrorCode.None;
        }
        catch (Exception)
        {
            return ErrorCode.DatabaseError;
        }
    }
}
