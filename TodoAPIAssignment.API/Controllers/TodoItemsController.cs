using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TodoAPIAssignment.API.Models.TodoItemsControllerModels.RequestModels;
using TodoAPIAssignment.AuthenticationLibrary;
using TodoAPIAssignment.AuthenticationLibrary.Models;
using TodoAPIAssignment.DataAccessLibrary;
using TodoAPIAssignment.DataAccessLibrary.Enums;
using TodoAPIAssignment.DataAccessLibrary.Models;
using TodoAPIAssignment.DataAccessLibrary.Models.Results.TodoItemResults;

namespace TodoAPIAssignment.API.Controllers;

[ApiController]
[Route("api/todos/{todoId}/items")]
public class TodoItemsController : ControllerBase
{
    private readonly ITodoItemDataAccess _todoItemDataAccess;
    private readonly IAuthenticationDataAccess _authenticationDataAccess;

    public TodoItemsController(ITodoItemDataAccess todoItemDataAccess, IAuthenticationDataAccess authenticationDataAccess)
    {
        _todoItemDataAccess = todoItemDataAccess;
        _authenticationDataAccess = authenticationDataAccess;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTodoItem(string todoId, [FromBody] CreateTodoItemRequestModel createTodoItemRequestModel)
    {
        try
        {
            string? token = ExtractTokenFromHeader(Request.Headers["Authorization"]!);
            if (token is null)
                return BadRequest(new { ErrorMessage = "InvalidAccessToken" });

            AppUser? appUser = await _authenticationDataAccess.CheckAndDecodeAccessTokenAsync(token);
            if (appUser is null)
                return BadRequest(new { ErrorMessage = "InvalidAccessToken" });

            TodoItem todoItem = new TodoItem()
            {
                Title = createTodoItemRequestModel.Title,
                Description = createTodoItemRequestModel.Description,
                IsDone = createTodoItemRequestModel.IsDone,
            };

            CreateTodoItemResult createTodoItemResult = await _todoItemDataAccess.CreateUserTodoItemAsync(appUser.Id!, todoId, todoItem);
            if (createTodoItemResult.ErrorCode == ErrorCode.TodoNotFound)
                return NotFound();
            else if (createTodoItemResult.ErrorCode == ErrorCode.DatabaseError)
                return StatusCode(500, new { ErrorMessage = "InternalServerError" });

            //TODO when I add the GetTodoById make this dynamic
            var locationUri = $"https://localhost:7279/api/todos/{todoId}/items/{createTodoItemResult.TodoItem!.Id}";
            return Created(locationUri, createTodoItemResult.TodoItem);

            //return CreatedAtAction(nameof(GetTodo), new { todoId = createTodoItemResult.Todo!.Id }, createTodoItemResult.Todo);
        }
        catch (Exception)
        {
            return StatusCode(500, new { ErrorMessage = "InternalServerError" });
        }
    }

    [HttpGet("{todoItemId}")]
    public async Task<IActionResult> GetTodoItem(string todoId, string todoItemId)
    {
        try
        {
            string? token = ExtractTokenFromHeader(Request.Headers["Authorization"]!);
            if (token is null)
                return BadRequest(new { ErrorMessage = "InvalidAccessToken" });

            AppUser? appUser = await _authenticationDataAccess.CheckAndDecodeAccessTokenAsync(token);
            if (appUser is null)
                return BadRequest(new { ErrorMessage = "InvalidAccessToken" });

            GetTodoItemResult getTodoItemResult = await _todoItemDataAccess.GetUserTodoItemAsync(appUser.Id!, todoId, todoItemId);
            if (getTodoItemResult.ErrorCode == ErrorCode.DatabaseError)
                return StatusCode(500, new { ErrorMessage = "InternalServerError" });
            else if (getTodoItemResult.ErrorCode == ErrorCode.TodoNotFound)
                return NotFound(new { ErrorMessage = "TodoNotFound" });
            else if (getTodoItemResult.ErrorCode == ErrorCode.TodoItemNotFound)
                return NotFound(new { ErrorMessage = "TodoItemNotFound" });

            return Ok(getTodoItemResult.TodoItem);
        }
        catch (Exception)
        {
            return StatusCode(500, new { ErrorMessage = "InternalServerError" });
        }
    }

    private string? ExtractTokenFromHeader(string header)
    {
        if (header.IsNullOrEmpty() || !header.StartsWith("Bearer "))
            return null;

        string token = header.Substring("Bearer ".Length).Trim();
        return token;
    }

}
