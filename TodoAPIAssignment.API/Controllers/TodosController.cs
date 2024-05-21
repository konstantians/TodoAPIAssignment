using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TodoAPIAssignment.API.Models.TodoControllerModels.RequestModels;
using TodoAPIAssignment.AuthenticationLibrary;
using TodoAPIAssignment.AuthenticationLibrary.Models;
using TodoAPIAssignment.DataAccessLibrary;
using TodoAPIAssignment.DataAccessLibrary.Enums;
using TodoAPIAssignment.DataAccessLibrary.Models;
using TodoAPIAssignment.DataAccessLibrary.Models.Results.TodoResults;

namespace TodoAPIAssignment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly ITodoDataAccess _todoDataAccess;
    private readonly IAuthenticationDataAccess _authenticationDataAccess;

    public TodosController(ITodoDataAccess todoDataAccess, IAuthenticationDataAccess authenticationDataAccess)
    {
        _todoDataAccess = todoDataAccess;
        _authenticationDataAccess = authenticationDataAccess;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTodo([FromBody] CreateTodoRequestModel createTodoRequestModel)
    {
        try
        {
            string? token = ExtractTokenFromHeader(Request.Headers["Authorization"]!);
            if (token is null)
                return BadRequest(new { ErrorMessage = "InvalidAccessToken" });

            AppUser? appUser = await _authenticationDataAccess.CheckAndDecodeAccessTokenAsync(token);
            if(appUser is null)
                return BadRequest(new { ErrorMessage = "InvalidAccessToken" });

            Todo todo = new Todo()
            {
                Title = createTodoRequestModel.Title,
                IsDone = createTodoRequestModel.IsDone,
                UserId = appUser?.Id
            };

            CreateTodoResult createTodoResult = await _todoDataAccess.CreateTodoAsync(todo);
            if(createTodoResult.ErrorCode == ErrorCode.DatabaseError)                
                return StatusCode(500, new { ErrorMessage = "InternalServerError" });

            //, 
            return CreatedAtAction(nameof(GetTodo), new { todoId = createTodoResult.Todo!.Id }, createTodoResult.Todo);
        }
        catch (Exception)
        {
            return StatusCode(500, new { ErrorMessage = "InternalServerError" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetTodos()
    {
        try
        {
            string? token = ExtractTokenFromHeader(Request.Headers["Authorization"]!);
            if (token is null)
                return BadRequest(new { ErrorMessage = "InvalidAccessToken" });

            AppUser? appUser = await _authenticationDataAccess.CheckAndDecodeAccessTokenAsync(token);
            if (appUser is null)
                return BadRequest(new { ErrorMessage = "InvalidAccessToken" });
            
            GetTodosResult getTodosResult = await _todoDataAccess.GetUserTodosAsync(appUser.Id!);
            if (getTodosResult.ErrorCode == ErrorCode.DatabaseError)
                return StatusCode(500, new { ErrorMessage = "InternalServerError" });

            return Ok(getTodosResult.Todos);
        }
        catch (Exception)
        {
            return StatusCode(500, new { ErrorMessage = "InternalServerError" });
        }
    }

    [HttpGet("{todoId}")]
    public async Task<IActionResult> GetTodo(string todoId)
    {
        try
        {
            string? token = ExtractTokenFromHeader(Request.Headers["Authorization"]!);
            if (token is null)
                return BadRequest(new { ErrorMessage = "InvalidAccessToken" });

            AppUser? appUser = await _authenticationDataAccess.CheckAndDecodeAccessTokenAsync(token);
            if (appUser is null)
                return BadRequest(new { ErrorMessage = "InvalidAccessToken" });

            GetTodoResult getTodoResult = await _todoDataAccess.GetUserTodoAsync(appUser.Id!, todoId);
            if (getTodoResult.ErrorCode == ErrorCode.DatabaseError)
                return StatusCode(500, new { ErrorMessage = "InternalServerError" });

            if (getTodoResult.ErrorCode == ErrorCode.TodoNotFound)
                return NotFound();

            return Ok(getTodoResult.Todo);
        }
        catch (Exception)
        {
            return StatusCode(500, new { ErrorMessage = "InternalServerError" });
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTodo([FromBody] UpdateTodoRequestModel updateTodoRequestModel)
    {
        try
        {
            string? token = ExtractTokenFromHeader(Request.Headers["Authorization"]!);
            if (token is null)
                return BadRequest(new { ErrorMessage = "InvalidAccessToken" });

            AppUser? appUser = await _authenticationDataAccess.CheckAndDecodeAccessTokenAsync(token);
            if (appUser is null)
                return BadRequest(new { ErrorMessage = "InvalidAccessToken" });

            Todo updatedTodo = new Todo()
            {
                Id = updateTodoRequestModel.Id,
                Title = updateTodoRequestModel.Title,
                IsDone = updateTodoRequestModel.IsDone,
                UserId = appUser.Id
            };

            UpdateTodoResult updateTodoResult = await _todoDataAccess.UpdateUserTodoAsync(updatedTodo);
            if (updateTodoResult.ErrorCode == ErrorCode.DatabaseError)
                return StatusCode(500, new { ErrorMessage = "InternalServerError" });

            if (updateTodoResult.ErrorCode == ErrorCode.TodoNotFound)
                return NotFound();

            return Ok(updateTodoResult.Todo);
        }
        catch (Exception)
        {
            return StatusCode(500, new { ErrorMessage = "InternalServerError" });
        }
    }

    [HttpDelete("{todoId}")]
    public async Task<IActionResult> DeleteTodo(string todoId)
    {
        try
        {
            string? token = ExtractTokenFromHeader(Request.Headers["Authorization"]!);
            if (token is null)
                return BadRequest(new { ErrorMessage = "InvalidAccessToken" });

            AppUser? appUser = await _authenticationDataAccess.CheckAndDecodeAccessTokenAsync(token);
            if (appUser is null)
                return BadRequest(new { ErrorMessage = "InvalidAccessToken" });

            ErrorCode errorCode = await _todoDataAccess.DeleteUserTodoAsync(appUser.Id!, todoId);
            if (errorCode == ErrorCode.DatabaseError)
                return StatusCode(500, new { ErrorMessage = "InternalServerError" });

            if (errorCode == ErrorCode.TodoNotFound)
                return NotFound();

            return NoContent();
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
