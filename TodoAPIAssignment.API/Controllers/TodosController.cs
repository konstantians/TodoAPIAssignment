﻿using Microsoft.AspNetCore.Mvc;
using TodoAPIAssignment.API.Models.TodoControllerModels.RequestModels;
using TodoAPIAssignment.AuthenticationLibrary;
using TodoAPIAssignment.AuthenticationLibrary.Models;
using TodoAPIAssignment.DataAccessLibrary;
using TodoAPIAssignment.DataAccessLibrary.Models;

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
            AppUser? appUser = await _authenticationDataAccess.CheckAndDecodeAccessTokenAsync(createTodoRequestModel.Token!);
            if(appUser is null)
                return BadRequest(new { ErrorMessage = "InvalidAccessToken" });

            Todo todo = new Todo()
            {
                Title = createTodoRequestModel.Title,
                IsDone = createTodoRequestModel.IsDone,
                UserId = appUser?.Id
            };

            CreateTodoResult createTodoResult = await _todoDataAccess.CreateTodoAsync(todo);
            if(createTodoResult.ErrorCode == DataAccessLibrary.Enums.ErrorCode.DatabaseError)                
                return StatusCode(500, new { ErrorMessage = "InternalServerError" });

            //TODO change that when I create the getTodoById method
            var locationUri = $"https://localhost:7279/api/todos/{createTodoResult.Todo!.Id}";
            return Created(locationUri, createTodoResult.Todo);
        }
        catch (Exception)
        {
            return StatusCode(500, new { ErrorMessage = "InternalServerError" });
        }
    }
}